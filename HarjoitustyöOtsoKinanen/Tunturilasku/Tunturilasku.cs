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


        GameObject tausta = new GameObject(1600, 6000);
        tausta.Image = LoadImage("Tunturilasku_tausta");
        tausta.Y = (-tausta.Height / 2 + Level.Height / 2);
        Add(tausta, -3);


        int nopeus = 1;


        if (tausta.Bottom >= Level.Bottom)
        {
            tausta.Destroy();
            GameObject uusiTausta = new GameObject(1600, 6000);
            uusiTausta.Image = LoadImage("Tunturilasku_tausta");
            //uusiTausta.Y = ((Screen.Height / 2) + (uusiTausta.Height / 2));
            Add(uusiTausta, -3);

        };

        

        nopeus = TaustaLiike(this, tausta, nopeus);


        PhysicsObject hahmo = new PhysicsObject(100, 200, Shape.Rectangle);
        hahmo.Image = LoadImage("hahmo"); ;
        Add(hahmo);
        hahmo.LinearDamping = 0.999;


        Ohjaus(this, hahmo);





        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    /*Tämä aliohjelma sisältää pelihahmon ohjauksen hiirellä ja nuolinäppäimillä.
    */
    private static void Ohjaus(Game peli, PhysicsObject hahmo)
    {
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

    //Aliohjelma pyörittää taustakuvaa, ja lisää taustan nopeutta, sekä palauttaa int arvona nopeuden pääohjelmaan. 
    private static int TaustaLiike (Game peli, GameObject tausta, int nopeus)
    {
        double kierros = 5;
                

        Timer taustaAjastin = new Timer();
        Timer taustanNopeus = new Timer();


        taustaAjastin.Interval = 0.01;


        taustanNopeus.Interval = kierros;
        taustanNopeus.Start();


        taustaAjastin.Start();
        taustaAjastin.Timeout += delegate (){ tausta.Y += nopeus; };


        taustanNopeus.Timeout += delegate () { nopeus++; };

 
        /*if ( tausta.Bottom >= peli.Level.Bottom )
        {
            tausta.Destroy();
            GameObject uusiTausta = new GameObject(1600, 6000);
            uusiTausta.Image = LoadImage("Tunturilasku_tausta");
            //uusiTausta.Y = ((Screen.Height / 2) + (uusiTausta.Height / 2));
            peli.Add(uusiTausta, -3);
            
        };
        */
        return nopeus;
    }





}


