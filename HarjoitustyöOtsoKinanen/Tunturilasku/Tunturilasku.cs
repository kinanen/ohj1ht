using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;


/// @author Otso Kinanen
/// @version 6.4.2022
///
/// <summary>
/// Laskettelupeli jossa ohjataan hiirellä, oikeaa nappia käyttäen hahmoa laskettelurinnettä turvallisesti alaspäin. Pelissä esteinä puita, kiviä ja ilves.
/// Osumista puihin menettää yhden elkun, osuma ilveksen kanssa tappaa kerrasta.
/// Osumalaskurin arvo palautuu pelin edetessä hitaasti alkuarvoonsa
///
/// </summary>
// LINKKI DEMO 7 Tehtäviin joissa Taulukko, Silmukka ja Toisto demoja. 
// 1. https://tim.jyu.fi/view/kurssit/tie/ohj1/2022k/demot/demo7#d3teht6a
// 2. https://tim.jyu.fi/view/kurssit/tie/ohj1/2022k/demot/demo7#d7t1
// 3. https://tim.jyu.fi/view/kurssit/tie/ohj1/2022k/demot/demo7#pituuksienKa
// 4.-5. https://tim.jyu.fi/view/kurssit/tie/ohj1/2022k/demot/demo7#d7teht4
// 6. https://tim.jyu.fi/view/kurssit/tie/ohj1/2022k/demot/demo7#d7teht5



public class Tunturilasku : PhysicsGame
{
    /// <summary>
    /// lukuarvo joka on pelattavan hahmon elämä(t) pelin alussa.
    /// </summary>
    private int elama = 3;

    /// <summary>
    /// pelaajan maksimi elämä arvo.
    /// </summary>
    private const int MAKSIMIELAMAT = 3;

    /// <summary>
    /// double-arvo "nopeus", joka määrittää kuinka nopeasti esteet tulevat rintessä vastaan. Aliohjelma Esteet muuttaa nopeus arvoa suuremmaksi pelin edetessä
    /// </summary>
    private double nopeus = 1;

    /// <summary>
    /// palkki joka kuvaa jäljellä olevaa elämiä int luvun arvoa pelaajalle.
    /// </summary>
    private DoubleMeter elamiaJaljella; // palkki joka kuvaa jäljellä olevaa elämiä int luvun arvoa pelaajalle.

    /// <summary>
    ///Ohjelman Begin, jossa määrätään pelin ikkunan ja kentän koko, lisätään rajat kenttään, joita pelaaja ei voi ylittää.
    ///Kutsutana taustakuva, pelin esteet, viholliset ja itse pelattava hahmo, sekä kutsutaan "Ohjaus" metodia, joka hoitaa hahmon liikuttamisen 
    ///lisätään törmäyskäsittelijä ja pelaajan elama arvoa parantava "paraneminen"
    ///Käynnistetään ja lopetetaan kaikki pelin tapahtumat "kuolemaan" ja annetaan pelaajalle pisteet.
    /// </summary>


    public override void Begin()
    {
        LuoKentta();

        LuoElamiaJaljella(); // kutsutaan aliohjelmaa joka luo elämäpalkin
        
        PhysicsObject hahmo = LisaaHahmo(this); // kutsutaan aliohjelmaa, joka palauttaa PhysicsObject:in joka toimii pelattavana hahmona argumettina peli johon hahmo lisätään.

        Ohjaus(this, hahmo); // kutsuu aliohjelmaa joka hoitaa pelaajan ohjauksen, argumenttina Game jossa ohjaus tapahtuu
        
        Ilves(this); // Kutsuu pelissä vihollisena olevien ilves olioiden aliohjelmaa, argumenttina peli johon aliohjelma lisää ilvekset

        Esteet(this, nopeus); // kutsutaan aliohjelmaa joka lisää peliin maasto esteet, argumentteina, pelin nopeus ja peli johon esteet lisätään.

        //Ilves(this); // Kutsuu pelissä vihollisena olevien ilves olioiden aliohjelmaa, argumenttina peli johon aliohjelma lisää ilvekset

        //Esteet(this, nopeus); // kutsutaan aliohjelmaa joka lisää peliin maasto esteet, argumentteina, pelin nopeus ja peli johon esteet lisätään.

        AddCollisionHandler(hahmo, Tormays); // törmäyskäsittelijä joka pienentää pelaajan elama arvoa sekä ylläpitää elamaPalkkia

        Pisteet();

        Paraneminen(); // parantaa ajastimella pelaajna elämät arvoa mikäli se on laskenut

        

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    ///<summary>Luodaan peliin kenttä, lisätään taustakuva ja määrätään sen koko</summary>
    private void LuoKentta()
    {
        Level.Size = new Vector(2000, 1400); //määrätään peli alueen koko
        Level.Background.Image = LoadImage("Tunturilasku_tausta1"); // lisätään peliin taustakuva
        Level.CreateBorders(); //  luodaan kenttään laidat
        Level.Size = new Vector(2000, 1400); //määrätään peli alueen koko

        SetWindowSize(1600, 900);// määrätään ikkunan koko

    }


    /// <summary>
    /// Tämä aliohjelma sisältää pelihahmon ohjauksen hiirellä ja nuolinäppäimillä.
    /// Ohjelma lataa pelaajan kuvan eri liikkeen mukaisesti, kuhunkin etenemis suuntaan oman kuvansa.
    /// </summary>
    /// <param name ="hahmo"> pelattava GameObject, jota ohjaus aliohjelma liikuttaa </param>
    /// <param name="peli" > peli jossa hahmon liikuttaminen tapahtuu </param>
    /// <returns></returns>
    private void Ohjaus(Game peli, PhysicsObject hahmo)
    {
        Image hahmonKuva = LoadImage("hahmo");
        Image hahmoVasenKuva = LoadImage("hahmo_vasen");
        Image hahmoOikeaKuva = LoadImage("hahmo_oikea");
        Image hahmoJarruKuva = LoadImage("hahmo_jarru");

        /// <summary>
        /// Aliohjelma, joka hoitaa varsinaisen pelaajan liikuttamisen käytettäessä nuolinäppäimiä
        /// </summary>
        /// <param name="suunta"> Vectori joka määrää liikkeen suunnan </param>
        /// <param name="kuva"> kuva joka määrittää että kuva vastaa liikeen suuntaa. </param>
        void LiikutaPelaajaa(Vector suunta, Image kuva)
        {
            hahmo.Push(suunta); // komento liikuttaa pelaajaa
            hahmo.Image = kuva; // muuttaa pelaajan kuvan

        }

        double[] nopeusTaulukko = new double[] { 150, 50, 30 }; // taulukko jossa on kerroin arvo jota käytetään pelaajan liikuttamiseen

        double nopeus = nopeusTaulukko[elama - 1]; // määrittää pelaajan liikkeen herkkyyttä/nopeutta perustuen "nopeusTaulukko" arvoihin ja pelaajan "elama" arvoon, käytetään pelaajan liike vectorin kertoimena

        peli.Mouse.Listen(MouseButton.Left, ButtonState.Down, delegate ()// hiiren kuuntelija joka ottaa huomioon hiiren ja pelaajan sijainnin, Mitä kauempana pelaajasta hiiren nappia painetaan, sitä voimakkaampi on liike siihen suuntaan. 
        {
            hahmo.Push((peli.Mouse.PositionOnWorld - hahmo.Position) * nopeus);
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
                
        peli.Mouse.Listen(MouseButton.Left, ButtonState.Released, delegate () //Palauttaa pelaajan hahmon "suoraan" lasku asentoon
        {
            hahmo.Image = hahmonKuva;
        }
        , null);


        //Pelaajalla on hiiriohjauksen lisäksi pelaajan mieltymyksen mukaan käytettävissä nuolinäppäimistöllä ohjaus. 
        peli.Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa liikutetaan oikealle", new Vector(6000, 0), hahmoOikeaKuva);
        peli.Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa liikutetaan oikealle", new Vector(-6000, 0), hahmoVasenKuva);
        peli.Keyboard.Listen(Key.Up, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa liikutetaan oikealle", new Vector(0, 6000), hahmoJarruKuva);
        peli.Keyboard.Listen(Key.Down, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa liikutetaan oikealle", new Vector(0, -6000), hahmonKuva);

    }


    ///<summary>Lisää peliin näkyvän sekuntikellon joka mittaa suorituksen kestoa.</summary>
    private void Pisteet()
    {
        Timer pistelaskuri = new Timer(); //luodaan ajastin "pistelaskuri"
        pistelaskuri.Start();

        pistelaskuri.Interval = 0.01;
        pistelaskuri.Timeout += delegate ()
        {
            if (elama < 1)
            {
                pistelaskuri.Stop();
            };
        };


        Label pistenaytto = new Label(); //lisätään laskuri näkyviin
        pistenaytto.TextColor = Color.Aquamarine; //määrätään laskurin väri
        pistenaytto.X =  250; // määrätään pistenäytön sijainti ruudulla
        pistenaytto.Y = 435;



        pistenaytto.BindTo(pistelaskuri.SecondCounter);
        Add(pistenaytto);

    }


    /// <summary>
    /// Aliohjelma joka lisää pelattavan hahmon peliin. Määrittää hahmon koon, kuvan, muodon kuvan perusteella, sekä sen liikkumiseen vaikuttavia arvoja
    /// Saa atribuuttina Game arvona pelin johon pelaaja lisätään, palauttaa pelaajan "PhysicisObject"ina.
    /// </summary>
    /// <param name="peli"> Tuodaan parametrinä Game johon GameObject lisätään </param>
    /// <returns> PhysicsObject "hahmo", joka toimii pelissä pelaajan hahmona </returns>
    private PhysicsObject LisaaHahmo(Game peli)
    {
        double pelaajaW = 75; // pelaaja leveys
        double pelaajaH = 150; // pelaaja korkeus
        Image kHahmo = LoadImage("hahmo"); // pelaajan kuva
        PhysicsObject hahmo = new PhysicsObject(pelaajaW, pelaajaH, Shape.FromImage(kHahmo));// luodaan pelaaja, ja annetaan sille muoto
        hahmo.Image = kHahmo; // lisää pelaajaan sen kuvan
        hahmo.CanRotate = false; // estää pelaatavaa hahmoa pyörimästä
        hahmo.Mass = 15; // pelaajan massa, joka vaikuttaa sen liikkeeseen ja törmäyksen käsittlyyn 
        peli.Add(hahmo); // lisätään pelaaja peliin
        hahmo.LinearDamping = 0.999;
        return hahmo;

    }


    /// <summary>
    /// Luodaan elamaPalkki, sekä määritetään sen ulkonäkö
    /// </summary>
    private void LuoElamiaJaljella()
    {
        elamiaJaljella = new DoubleMeter(elama); // elama arvon mukainen elamaPalkkiin aloitus arvo
        elamiaJaljella.MaxValue = MAKSIMIELAMAT;    //pelaajan maksimi elama healtPalkkiissa
        elamiaJaljella.LowerLimit += KuolemaPalkki; // käsittelijä sille tilanteelle jossa pelaajan elama on laskenut nollaan, kutsutaan KuolemaPalkki aliohjelmaa

        ProgressBar elamaPalkki = new ProgressBar(500, 10); // määrätään elamaPalkkiin koko 
        elamaPalkki.BarColor = Color.Aquamarine; // määrätään elamaPalkkiin väri

        elamaPalkki.X = Screen.Right - 260; // määrätään elamaPalkkiin sijainti ruudulla
        elamaPalkki.Y = Screen.Top - 20;
        elamaPalkki.BindTo(elamiaJaljella); // linkittää näkyvän palkin arvon elamaleft arvoon. 
        Add(elamaPalkki); // lisää elamaPalkkiin ruutuun

    }


    /// <summary>
    /// Näyttää pelaajalle elamaPalkkiin paikalla tekstin, mikäli pelaaja menettää kaikki elama pisteensä
    /// </summary>
    private void KuolemaPalkki()
    {
        MessageDisplay.Add("Humps ja Tumps, laskijasi menehtyi");

    }


    /// <summary>
    /// Aliohjelma ylläpitää ajastinta joka palauttaa pelaajan menetettyjä elama arvoja, ja kutsuu itseään niin kauan kun arvo on alle 3
    /// </summary>
    private void Paraneminen()
    {
        Timer.CreateAndStart(10, Paraneminen);

        if (elama < MAKSIMIELAMAT)
        {
            elama++;
            elamiaJaljella.Value++;
            
        }
    
    }


    /// <summary>
    /// Törmäyskäsittelijä joka vähentää osuman kohteen mukaan pelaajan elamaia, ja sen muuttuessa nollaan tuhoaa pelaajan. 
    /// </summary>
    /// <param name="hahmo"> pelaajan hahmo </param>
    /// <param name="kohde"> törmäyksen kohde </param>
    private void Tormays(PhysicsObject pelaaja, PhysicsObject kohde)
    {
        if (kohde.Tag == "ilves") // tarkistaa onko törmäyksen kohteella Tag "ilves"
        {
            pelaaja.Destroy(); // tuhoaa pelaajan kerta osumasta
            elama = 0;
            elamiaJaljella.Value = 0; // muuttaa elamaPalkkiin arvon
        }
        else // käsittelee törmäyksen jossa osapuolena ei ole ilves, vaan joku muu este tai kentän raja.
        {
            elama--;
            elamiaJaljella.Value--;
        }
        
        kohde.Destroy();
        if(elama <= 0)
        {
            pelaaja.Destroy();
            elamiaJaljella.Value = 0;
                        
        }
        
    }


    /// <summary>
    /// Aliohjelma hoitaa esteitä lisäävän LuoEsteet aliohjelman kutsumista ja ajastamista, sekä ylläpitää muuttujia joiden arvot määrittävät niiden nopeutta ja tiheyttä
    /// </summary>
    /// <param name="peli"> Tuodaan parametrinä Game peli johon GameObject lisätään</param>
    /// <param name="nopeus"> double nopeus, joka välitetään edelleen kutsuttavalle sekä jonka arvoa lisätään ajastimen Timeout:issa  </param>
    private void Esteet(Game peli, double nopeus)
    {
        double esteNopeus = 0.25; //lukuarvo "esteNopeus" joka määrittää kuinka tiheästi pelin alkaessa esteitä luodaan kentälle. 
        Timer ajastin = new Timer(); 
        ajastin.Interval = esteNopeus;

        ajastin.Timeout += delegate () { int rnd = RandomGen.NextInt(0, 4); LuoEsteet(peli, rnd, nopeus); esteNopeus=esteNopeus * 0.95; nopeus = nopeus * 1.005; };
        ajastin.Start();
        if(elama < 1) { ajastin.Stop(); }

    }


    /// <summary>
    /// Luo taulukon pelissä esiintyvistä esteistä ja lisää niitä, lisää niiden kuvat, sekä ylläpitää niiden liikettä.
    /// </summary>
    /// <param name="peli">Tuodaan parametrinä Game johon GameObject lisätään</param>
    /// <param name="a">int a jossa on taulukon indeksi josta lisättävä PhysicsObject löytyy</param>
    /// <param name="nopeus">double nopeus, jota käytetään määräämään lisättävän objektin liikkeen nopeutta.</param>
    private static void LuoEsteet(Game peli, int a, double nopeus)
    {
        PhysicsObject[] esteet = new PhysicsObject[4];


        Image kivi1k = LoadImage("kivi1");
        Image puu2k = LoadImage("puu2");
        Image puu1k = LoadImage("puu1");
        Image kivi2k = LoadImage("kivi2");


        PhysicsObject puu1 = new PhysicsObject(100, 200, Shape.Triangle);
        puu1.Image = puu1k;
        esteet[0] = puu1;

        PhysicsObject puu2 = new PhysicsObject(100, 200, Shape.Triangle);
        puu2.Image = puu2k;
        esteet[1] = puu2;

        PhysicsObject kivi1 = new PhysicsObject(100, 50, Shape.Triangle);
        kivi1.Image = kivi1k;
        esteet[2] = kivi1;

        
        PhysicsObject kivi2 = new PhysicsObject(100, 50, Shape.Triangle);

        kivi2.Image = kivi2k;
        esteet[3] = kivi2;

        double x = RandomGen.NextDouble(-800, 800);
        esteet[a].X = x;
        esteet[a].Y = -650;
        peli.Add(esteet[a]);
        Vector xy = new Vector(x, 500);
        esteet[a].Mass = 150;
        esteet[a].MoveTo(xy, 500 * nopeus, esteet[a].Destroy);

    }


    /// <summary>
    /// Aliohjelma joka hoitaa ilvesten ajoituksen pelissä, sekä kutsuu niitä lisäävän aliohjelman         
    /// </summary>
    /// <param name="peli"> Tuodaan parametrinä Game johon GameObject lisätään </param>
    private static void Ilves (Game peli)
    {
        double a = 10;
        Timer ajastin = new Timer();
        ajastin.Interval = a;
        ajastin.Timeout += delegate () { LuoIlves(peli); a = a * 0.8 ; };
        ajastin.Start();

    }


    /// <summary>
    /// Lataa vihollis ilveksen arvot, koon, ominaisuudet arpoo RandomGen metodilla arvon 0-1 jolla määrätään suunta josta ilves tulee peliin.
    /// Aliohjelma myös tuhoaa ilveksen kun se on kulkenut ruudun läpi määrättyyn pitsteeseen.
    /// </summary>
    /// <param name="peli"> Tuodaan parametrinä Game johon GameObject lisätään </param>
    private static void LuoIlves(Game peli)
    {
        int suunta = RandomGen.NextInt(0, 2);//random gen joka arpoo kummalle puolelle ilves tulee
        PhysicsObject ilves = new PhysicsObject(100, 50);
        ilves.Tag = "ilves";
        Vector d = new(0, RandomGen.NextDouble(-450, 450));
        ilves.Y = RandomGen.NextInt(-450, 450);

        if (suunta == 0) //luodaan ilves, mikäli arvo on nolla, jolloin etenemissuunta on vasemmalta oikealle
        {
            ilves.X = -800;
            Image kIlvesvo = LoadImage("ilvesvo");
            Shape mIlves = Shape.FromImage(kIlvesvo);
            ilves.Shape = mIlves;
            ilves.Image = (kIlvesvo);
            d.X = 800.00;
            
        }
        else // luodaan ilves, mikäli suunta arvo on 1, jolloin ilveksen etenemissuunta on oikealta vasemmaelle.
        {
            ilves.X = 800;
            Image kIlvesov = LoadImage("ilvesov");
            Shape mIlves = Shape.FromImage(kIlvesov);
            ilves.Shape = mIlves;
            ilves.Image = (kIlvesov);
            d.X = -800.00;
        }

        ilves.MoveTo(d, 1000, ilves.Destroy);
        peli.Add(ilves);
    }


}


