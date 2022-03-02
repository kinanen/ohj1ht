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
        GameObject tausta = new GameObject(1600, 900);
        tausta.Image = LoadImage("Tunturilasku_tausta");
        Add(tausta);
        
        Image hahmonKuva = LoadImage("hahmo");
        Image hahmoVasenKuva = LoadImage("hahmo_vasen");
        Image hahmoOikeaKuva = LoadImage("hahmo_oikea");
        Image hahmoJarruKuva = LoadImage("hahmo_jarru");

        PhysicsObject hahmo = new PhysicsObject(100, 200, Shape.Rectangle);
        hahmo.Image = LoadImage("hahmo");
        Add(hahmo);
        hahmo.LinearDamping = 0.999;

        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaaO, "Pelaajaa liikutetaan oikealle", new Vector(1000, 0));
        Mouse.Listen(MouseButton.Left, ButtonState.Down, delegate ()
          {
              hahmo.Push((Mouse.PositionOnWorld - hahmo.Position) * 6);
              if (hahmo.Position.Y - Mouse.PositionOnWorld.Y < 0)
              {
                  hahmo.Image = LoadImage("hahmo_jarru");
              }
              else if(hahmo.Position.X - Mouse.PositionOnWorld.X < 0)
              {
                  hahmo.Image = LoadImage("hahmo_oikea");
              }
              else
              {
                  hahmo.Image = LoadImage("hahmo_vasen");
              }

          }
           , null);
        Mouse.Listen(MouseButton.Left, ButtonState.Released, delegate ()
             {
                 hahmo.Image = LoadImage("hahmo");
    ;
             }
        , null);

        void LiikutaPelaajaaO(Vector vector)
        {
            hahmo.Push(vector);
        }




        /*        List<PhysicsObject> esteet = new List<PhysicsObject>();

                LisaaEste();

                List<PhysicsObject> viholliset = new List<PhysicsObject>();

                LisaaVihollinen();
        */


        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

}


