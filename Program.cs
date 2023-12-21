using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SvetskaPrvenstvaApp
{
    class Program
    {
        static Dictionary<string, Država> države = new Dictionary<string, Država>();
        static string filePathDržave = "drzave.csv";
        static string filePathPrvenstva = "prvenstva.csv";

        static void Main(string[] args)
        {
            UčitajPodatkeIzFajla();

            bool kraj = false;
            while (!kraj)
            {
                Console.WriteLine("\n1. Prikaz svih država");
                Console.WriteLine("2. Prikaz svih svetskih prvenstava");
                Console.WriteLine("3. Unos nove države");
                Console.WriteLine("4. Unos novog svetskog prvenstva");
                Console.WriteLine("5. Sortiranje država po nazivu");
                Console.WriteLine("6. Sortiranje prvenstava po nazivu");
                Console.WriteLine("7. Prikaz prvenstava u opsegu godina");
                Console.WriteLine("8. Izlaz");

                Console.Write("\nIzaberite opciju: ");
                string izbor = Console.ReadLine();

                switch (izbor)
                {
                    case "1":
                        PrikazSvihDržava();
                        break;
                    case "2":
                        PrikazSvihPrvenstava();
                        break;
                    case "3":
                        UnosNoveDržave();
                        break;
                    case "4":
                        UnosNovogPrvenstva();
                        break;
                    case "5":
                        SortirajDržavePoNazivu();
                        break;
                    case "6":
                        SortirajPrvenstvaPoNazivu();
                        break;
                    case "7":
                        PrikazPrvenstavaPoOpseguGodina();
                        break;
                    case "8":
                        kraj = true;
                        break;
                    default:
                        Console.WriteLine("Nepoznata komanda.");
                        break;
                }
            }

            SačuvajPodatkeUFajl();
        }

        static void PrikazSvihDržava()
        {
            foreach (var država in države)
            {
                Console.WriteLine($"Država: {država.Value.Naziv}, Glavni grad: {država.Value.GlavniGrad}");
            }
        }

        static void PrikazSvihPrvenstava()
        {
            foreach (var država in države)
            {
                foreach (var prvenstvo in država.Value.SvetskaPrvenstva)
                {
                    Console.WriteLine($"Prvenstvo: {prvenstvo.Naziv}, Godina: {prvenstvo.GodinaOdržavanja}, Domaćin: {prvenstvo.Domaćin.Naziv}");
                }
            }
        }

        static void UnosNoveDržave()
        {
            Console.Write("Unesite naziv države: ");
            string naziv = Console.ReadLine();
            Console.Write("Unesite glavni grad: ");
            string grad = Console.ReadLine();

            var novaDržava = new Država(naziv, grad);
            države.Add(naziv, novaDržava);
            Console.WriteLine("Država je dodata.");
            SačuvajPodatkeUFajl();

        }

        static void UnosNovogPrvenstva()
        {
            Console.Write("Unesite naziv prvenstva: ");
            string naziv = Console.ReadLine();
            Console.Write("Unesite godinu održavanja: ");
            int godina = int.Parse(Console.ReadLine());
            Console.Write("Unesite naziv države domaćina: ");
            string nazivDržave = Console.ReadLine();

            if (države.TryGetValue(nazivDržave, out Država domaćin))
            {
                var novoPrvenstvo = new SvetskoPrvenstvo(naziv, godina, domaćin);
                domaćin.SvetskaPrvenstva.Add(novoPrvenstvo);
                Console.WriteLine("Prvenstvo je dodato.");
            }
            else
            {
                Console.WriteLine("Navedena država ne postoji!");
            }
            SačuvajPodatkeUFajl();

        }

        static void SortirajDržavePoNazivu()
        {
            var sortiraneDržave = države.Values.OrderBy(d => d.Naziv).ToList();
            foreach (var država in sortiraneDržave)
            {
                Console.WriteLine($"Država: {država.Naziv}, Glavni grad: {država.GlavniGrad}");
            }
        }

        static void SortirajPrvenstvaPoNazivu()
        {
            var sortiranaPrvenstva = new List<SvetskoPrvenstvo>();
            foreach (var država in države.Values)
            {
                sortiranaPrvenstva.AddRange(država.SvetskaPrvenstva);
            }
            sortiranaPrvenstva = sortiranaPrvenstva.OrderBy(p => p.Naziv).ToList();

            foreach (var prvenstvo in sortiranaPrvenstva)
            {
                Console.WriteLine($"Prvenstvo: {prvenstvo.Naziv}, Godina: {prvenstvo.GodinaOdržavanja}, Domaćin: {prvenstvo.Domaćin.Naziv}");
            }
        }

        static void PrikazPrvenstavaPoOpseguGodina()
        {
            Console.Write("Unesite početnu godinu: ");
            int početnaGodina = int.Parse(Console.ReadLine());
            Console.Write("Unesite krajnju godinu: ");
            int krajnjaGodina = int.Parse(Console.ReadLine());

            var prvenstvaUOpsegu = new List<SvetskoPrvenstvo>();
            foreach (var država in države.Values)
            {
                prvenstvaUOpsegu.AddRange(država.SvetskaPrvenstva.Where(p => p.GodinaOdržavanja >= početnaGodina && p.GodinaOdržavanja <= krajnjaGodina));
            }

            foreach (var prvenstvo in prvenstvaUOpsegu)
            {
                Console.WriteLine($"Prvenstvo: {prvenstvo.Naziv}, Godina: {prvenstvo.GodinaOdržavanja}, Domaćin: {prvenstvo.Domaćin.Naziv}");
            }
        }

        static void UčitajPodatkeIzFajla()
        {
            if (File.Exists(filePathDržave))
            {
                var linijeDržava = File.ReadAllLines(filePathDržave);
                foreach (var linija in linijeDržava)
                {
                    var podaci = linija.Split(',');
                    var država = new Država(podaci[0], podaci[1]);
                    države.Add(država.Naziv, država);
                }
            }

            if (File.Exists(filePathPrvenstva))
            {
                var linijePrvenstva = File.ReadAllLines(filePathPrvenstva);
                foreach (var linija in linijePrvenstva)
                {
                    var podaci = linija.Split(',');
                    if (države.TryGetValue(podaci[2], out Država domaćin))
                    {
                        var prvenstvo = new SvetskoPrvenstvo(podaci[0], int.Parse(podaci[1]), domaćin);
                        domaćin.SvetskaPrvenstva.Add(prvenstvo);
                    }
                }
            }
        }

        static void SačuvajPodatkeUFajl()
        {
            var linijeDržava = države.Values.Select(d => $"{d.Naziv},{d.GlavniGrad}").ToArray();
            File.WriteAllLines(filePathDržave, linijeDržava);

            var linijePrvenstva = new List<string>();
            foreach (var država in države.Values)
            {
                linijePrvenstva.AddRange(država.SvetskaPrvenstva.Select(p => $"{p.Naziv},{p.GodinaOdržavanja},{p.Domaćin.Naziv}"));
            }
            File.WriteAllLines(filePathPrvenstva, linijePrvenstva.ToArray());
        }

        class Država
        {
            public string Naziv { get; set; }
            public string GlavniGrad { get; set; }
            public List<SvetskoPrvenstvo> SvetskaPrvenstva { get; set; }

            public Država(string naziv, string glavniGrad)
            {
                Naziv = naziv;
                GlavniGrad = glavniGrad;
                SvetskaPrvenstva = new List<SvetskoPrvenstvo>();
            }
        }

        class SvetskoPrvenstvo
        {
            public string Naziv { get; set; }
            public int GodinaOdržavanja { get; set; }
            public Država Domaćin { get; set; }

            public SvetskoPrvenstvo(string naziv, int godinaOdržavanja, Država domaćin)
            {
                Naziv = naziv;
                GodinaOdržavanja = godinaOdržavanja;
                Domaćin = domaćin;
            }
        }
    }
}
