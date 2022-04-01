using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Tunturilasku : PhysicsGame
{


    public override void Begin()
    {
        // Kirjoita ohjelmakoodisi tähän
        Level.Size = new Vector(1600, 900);
        SetWindowSize(1600, 900);

        int nopeus = 1;
      
        PhysicsObject hahmo = LisaaHahmo(this);
        

        Ohjaus(this, hahmo);

        PhysicsObject ilves = Ilves( this, nopeus);
        PhysicsObject[] esteet = Esteet(this);
        //AddCollisionHandler(hahmo);

      
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    private static PhysicsObject[] Esteet(Game peli)
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
        esteet[1]=puu2;

        Image kivi1k = LoadImage("kivi1");
        Image kivi1s = LoadImage("kivi1shape");
        PhysicsObject kivi1 = new PhysicsObject(200, 100, Shape.FromImage(kivi1s));
        esteet[2] = kivi1;

        Image kivi2k = LoadImage("kivi2");
        Image kivi2s = LoadImage("kivi2shape");
        PhysicsObject kivi2 = new PhysicsObject(200, 100, Shape.FromImage(kivi2s));
        esteet[3] = kivi2;

        return esteet;

    }
    

    /*private static void Tausta(Game peli)
    {
        Ga
        LuoTaustakuvat();
        LiikutaTaustaa();
 
    }
    */

        private static PhysicsObject Ilves (Game peli, int nopeus)
    {
        int a = 10;
        Image kIlvesvo = LoadImage("ilvesvo");
        Shape mIlves = Shape.FromImage(kIlvesvo);
        PhysicsObject ilves = new PhysicsObject(200, 100, mIlves);
        ilves.Image = (kIlvesvo);
        Timer ajastin = new Timer();
        ajastin.Interval = a;
        ajastin.Timeout += delegate () { peli.Add(ilves); a++; };//tähän uusi LuoIlves aliohjelma, jossa luodaan aina uusi ilves!
        ajastin.Start();

        Timer poisto = new Timer();
        poisto.Interval = a+5;
        poisto.Timeout += delegate () { ilves.Destroy(); };
        poisto.Start();
        return ilves;


    }

    /*private static void IlvesTormays(GameObject pelaaja)
    {
             pelaaja.Destroy();
    }


    private static void TormausKasittelija(PhysicsObject pelajaa, PhysicsObject este)
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


