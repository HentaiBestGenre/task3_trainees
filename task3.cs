using System;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApplication3
{
    class Program
    {
        public class Rools{
            public List<List<string>> Graf = new List<List<string>>();
            public string[] gameElements;
            public Rools(string[] gameElements){
                this.gameElements = gameElements;
                int Len = gameElements.Length; 
                int offset = Len/2;
                for (int line = 0; line < Len; ++line){
                    this.Graf.Add(new List<string>());
                    for (int row = 0; row < Len; ++row){
                        if (line == row){ this.Graf[line].Add("drow");}
                        else if (line - row > offset || (row - line > 0 && row - line <= offset))
                            { this.Graf[line].Add(gameElements[line]);}
                        else if (line - row <= offset || row - line > offset)
                            { this.Graf[line].Add(gameElements[row]);}
                    }
                }
            }

            public void showMenu(){
                for(int i = 0; i < this.gameElements.Length; ++i){
                    Console.WriteLine($"{i+1} - {this.gameElements[i]}");
                }
                Console.WriteLine("0 - exit");
                Console.WriteLine("? - help");
            }

            public string IsWin(int userMI, int PCmove){
                var res = this.Graf[userMI][PCmove];
                if(res == this.gameElements[userMI]){ return "you win"; }
                else if(res == this.gameElements[PCmove]){ return "you lose"; }
                else{return "drow";}
            }
        
            public void Help(){
                var maxWidth = this.gameElements.Max(s => s.Length);
                int Len = this.gameElements.Length;
                var formatString = string.Format("{{0, -{0}}}|", maxWidth);
                Console.Write(formatString, " ");
                foreach(var i in this.gameElements){Console.Write(formatString, i);}
                Console.WriteLine();
                for (int line = 0; line < Len; ++line){
                    Console.Write(formatString, this.gameElements[line]);
                    foreach (var i in this.Graf[line]){ Console.Write(formatString, i); }
                    Console.WriteLine();
                }
            }
        
        }
        public class Hmac{
            private Byte[] keyBytes, hmacBytes;
            public Hmac(string move){
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                ASCIIEncoding encoding = new ASCIIEncoding();
                HMACSHA256 hmacsha256;
                Byte[] moveBytes = encoding.GetBytes(move);
                this.keyBytes = new Byte[32];
                rng.GetBytes(keyBytes);
                hmacsha256 = new HMACSHA256(keyBytes);
                this.hmacBytes = hmacsha256.ComputeHash(moveBytes);
            }
            public void ShowKey() => Console.WriteLine(BitConverter.ToString(this.keyBytes).Replace("-", "").ToLower());
            public void ShowHmac() => Console.WriteLine(BitConverter.ToString(this.hmacBytes).Replace("-", "").ToLower());
        }

        static bool tests(string[] gameElements){
            System.Console.WriteLine("\t------------------------------------------");
            if(gameElements.Length < 3 || gameElements.Length%2 == 0){
                System.Console.WriteLine("\tERROR!\n\tThe number of turns must be 3 or more and odd.");
                System.Console.WriteLine("\t------------------------------------------");
                return false;
            }
            foreach(string i in gameElements){
                if (gameElements.Where(x => x == i).Count() > 1){
                    System.Console.WriteLine("\tERROR!\n\tThe elements of the game must be different.");
                    System.Console.WriteLine("\t------------------------------------------");
                    return false;
                }
            }
            return true;
        }

        static void Main(string[] gameElements){
            if(!tests(gameElements)){ System.Environment.Exit(0); }
            Rools rools = new Rools(gameElements);
            Random rand = new Random();
            int PCmoveId = rand.Next(0, gameElements.Length);
            string PCmove = gameElements[PCmoveId];
            Hmac hmac = new Hmac(PCmove);
            hmac.ShowKey();
            while(true){
                rools.showMenu();
                Console.Write("Enter your move:");
                string? playerMove = Console.ReadLine();
                try{
                    int MoveId = int.Parse(playerMove!) - 1;
                    if (MoveId >= 0 && MoveId < gameElements.Length){
                        Console.WriteLine(rools.IsWin(MoveId, PCmoveId)); 
                        Console.WriteLine($"your move - {gameElements[MoveId]}");
                        Console.WriteLine($"PC move - {gameElements[PCmoveId]}");
                        hmac.ShowHmac();
                        break;}
                    else if (MoveId == -1) { System.Environment.Exit(-1); }
                }catch{ if (playerMove == "?"){rools.Help();} }
            }
        }
    }
}
