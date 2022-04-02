using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Tunturilasku : PhysicsGame
{

    int health = 5;


    public override void Begin()
    {
        // Kirjoita ohjelmakoodisi tähän
        Level.Size = new Vector(1600, 900);
        SetWindowSize(1600, 900);

        int nopeus = 1;

        PhysicsObject hahmo = LisaaHahmo(this);

        Ohjaus(this, hahmo);

        Ilves(this, nopeus);

        Esteet(this, nopeus);

        Tausta(this, nopeus);


        AddCollisionHandler(hahmo, Tormays);


        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    void Tormays(PhysicsObject pelaaja, PhysicsObject kohde)
    {
        health--;

        /* if(health <= 0)
        {
            hahmo.Destroy;
        }
        */
    }

    private static void Esteet(Game peli, int b)
    {
        double a = 0.5;

        Timer ajastin = new Timer();
        ajastin.Interval = a;

        ajastin.Timeout += delegate () { int rnd = RandomGen.NextInt(0, 4); LuoEsteet(peli, rnd); a = a - 0.05; };
        ajastin.Start();
    }

    private static void LuoEsteet(Game peli, int a)
    {
        PhysicsObject[] esteet = new PhysicsObject[4];

        Image puu1k = LoadImage("puu1");
        Image puu1s = LoadImage("puu1shape");
        PhysicsObject puu1 = new PhysicsObject(100, 200, Shape.FromImage(puu1s));
        puu1.Image = puu1k;
        esteet[0] = puu1;

        Image puu2k = LoadImage("puu2");
        Image puu2s = LoadImage("puu2shape");
        PhysicsObject puu2 = new PhysicsObject(100, 200, Shape.FromImage(puu2s));
        puu2.Image = puu2k;
        esteet[1] = puu2;

        Image kivi1k = LoadImage("kivi1");
        Image kivi1s = LoadImage("kivi1shape");
        PhysicsObject kivi1 = new PhysicsObject(200, 100, Shape.FromImage(kivi1s));
        kivi1.Image = kivi1k;
        esteet[2] = kivi1;

        Image kivi2k = LoadImage("kivi2");
        Image kivi2s = LoadImage("kivi2shape");
        PhysicsObject kivi2 = new PhysicsObject(200, 100, Shape.FromImage(kivi2s));
        kivi2.Image = kivi2k;
        esteet[3] = kivi2;

        double x = RandomGen.NextDouble(-800, 800);
        esteet[a].X = x;
        esteet[a].Y = -650;
        peli.Add(esteet[a]);
        Vector xy = new Vector(x, 500);
        esteet[a].Mass = 150;
        esteet[a].MoveTo(xy, 500, esteet[a].Destroy);
    }

    private static void Tausta(Game peli, int nopeus)
    {
        LuoTausta(peli);

    }


    private static void LuoTausta(Game peli)
    {

        Image ktausta = LoadImage("Tunturilasku_tausta1");

        GameObject tausta = new GameObject(1600, 6000);
        tausta.Image = ktausta;
        tausta.Y = -2600;
        peli.Add(tausta, -3);
        Vector up = new Vector(0, 2600);
        tausta.MoveTo(up, 500, delegate () { LuoTausta(peli); });       


    }
    


    private static void Ilves (Game peli, int nopeus)
    {
        int a = 10;
        Timer ajastin = new Timer();
        ajastin.Interval = a;
        ajastin.Timeout += delegate () { LuoIlves(peli, a); a++; };
        ajastin.Start();
                
    }


    private static void LuoIlves(Game peli, int a)
    {
        //random gen joka arpoo kummalle puolelle ilves tulee, X = 800 tai -800 Y satunnainen välillä -450 ja 450, mieti tuleeko kohti , ristiin vai satunnaiseen kohtaan
        int suunta = RandomGen.NextInt(0, 2);

        if(suunta == 0)
        {
            int x = -800;
            Image kIlvesvo = LoadImage("ilvesvo");
            Shape mIlves = Shape.FromImage(kIlvesvo);
            PhysicsObject ilves = new PhysicsObject(200, 100, mIlves);
            ilves.Image = (kIlvesvo);
            ilves.X = x;
            ilves.Y = RandomGen.NextInt(-450, 450);
            peli.Add(ilves);

            Vector d = new Vector(800.00, RandomGen.NextDouble(-450, 450));
            ilves.MoveTo(d, 1000, ilves.Destroy);

        }
        else
        {
            int x = 800;
            Image kIlvesov = LoadImage("ilvesov");
            Shape mIlves = Shape.FromImage(kIlvesov);
            PhysicsObject ilves = new PhysicsObject(200, 100, mIlves);
            ilves.Image = (kIlvesov);
            ilves.X = x;
            ilves.Y = RandomGen.NextInt(-450, 450);

            peli.Add(ilves);

            Vector d = new Vector(-800.00, RandomGen.NextDouble(-450, 450));
            ilves.MoveTo(d, 1000, ilves.Destroy);
            
        }

        


        

    }


   /* private static void TormausKasittelija(PhysicsObject pelajaa, PhysicsObject este)
    {
        
    }*/
        

    private static PhysicsObject LisaaHahmo(Game peli)
    {
        Image kHahmo = LoadImage("hahmo");
        PhysicsObject hahmo = new PhysicsObject(100, 200, Shape.FromImage(kHahmo) );
        hahmo.Image = kHahmo;
        hahmo.CanRotate = false;
        hahmo.Mass = 15;
        peli.Add(hahmo);
        hahmo.LinearDamping = 0.999;

        return hahmo;
        

    }


    private static void Ohjaus(Game peli, PhysicsObject hahmo)
    {
        //Tämä aliohjelma sisältää pelihahmon ohjauksen hiirellä ja nuolinäppäimillä.
    
        Image hahmonKuva = LoadImage("hahmo");
        Image hahmoVasenKuva = LoadImage("hahmo_vasen");
        Image hahmoOikeaKuva = LoadImage("hahmo_oikea");
        Image hahmoJarruKuva = LoadImage("hahmo_jarru");


        peli.Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa liikutetaan oikealle", new Vector(6000, 0), hahmoOikeaKuva);
        peli.Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa liikutetaan oikealle", new Vector(-6000, 0), hahmoVasenKuva);
        peli.Keyboard.Listen(Key.Up, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa liikutetaan oikealle", new Vector(0, 6000), hahmoJarruKuva);
        peli.Keyboard.Listen(Key.Down, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa liikutetaan oikealle", new Vector(0, -6000), hahmonKuva);


        void LiikutaPelaajaa(Vector vector, Image kuva)
        {
            hahmo.Push(vector);
            hahmo.Image = kuva;
        }


        peli.Mouse.Listen(MouseButton.Left, ButtonState.Down, delegate ()
        {
            hahmo.Push((peli.Mouse.PositionOnWorld - hahmo.Position) * 20);
            if (hahmo.Position.Y - peli.Mouse.PositionOnWorld.Y < 0)
            {
                hahmo.Image = hahmoJarruKuva;
            }
            else if (hahmo.Position.X - peli.Mouse.PositionOnWorld.X < 0)
            {
                hahmo.Image = hahmoOikeaKuva;
            }
            else
            {
                hahmo.Image = hahmoVasenKuva;
            }

        }
           , null);

        //Palauttaa pelaajan hahmon "suoraan" lasku asentoon
        peli.Mouse.Listen(MouseButton.Left, ButtonState.Released, delegate ()
        {
            hahmo.Image = hahmonKuva;
        }
        , null);
    }


}


