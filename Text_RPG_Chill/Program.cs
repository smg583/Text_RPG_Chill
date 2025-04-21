using System.Reflection.Metadata.Ecma335;
using static TexTRPG_Team_ver.Program;

namespace TexTRPG_Team_ver
{
    internal class Program
    {
        public class Character
        {
            public string name;
            public string job;
            public int level = 1;
            public int atk = 10;
            public int def = 5;
            public int hp = 100;
            public int gold = 1500;
        }

        public class Monster
        {
            public string name;
            public int level;
            public int hp;
            public int atk;
            public bool isDead = false;

        }

        static void Main(string[] args)
        {
            Character character = new Character();

            List<Monster> monsters = new List<Monster>();
            monsters.Add(new Monster { name = "미니언", level = 2, hp = 15, atk = 5 });
            monsters.Add(new Monster { name = "공허충", level = 3, hp = 10, atk = 9 });
            monsters.Add(new Monster { name = "대포미니언", level = 5, hp = 25, atk = 8 });

            CreatCharacter(character);
            MainMenu(character, monsters);
        }

        public static void CreatCharacter(Character character)
        {
            Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.\n");
            
            Console.WriteLine("원하시는 이름을 설정해 주세요: ");
            Console.Write(">>> ");
            character.name = Console.ReadLine();
            Console.WriteLine(" ");
            
            Console.WriteLine("해당 캐릭터의 직업을 설정해 주세요.(숫자만 입력)");
            Console.WriteLine("1. 전사\n2. 도적\n3. 팔라딘");
            Console.Write(">>> ");
            string action = Console.ReadLine();

            switch (action)
            {
                case "1":
                    character.job = "전사";
                    break;
                case "2":
                    character.job = "도적";
                    break;
                case "3":
                    character.job = "팔라딘";
                    break;
            }
            
        }


        public static void MainMenu(Character character, List<Monster> monsters)
        {
            do
            {
                Console.Clear();
                Console.WriteLine("메인메뉴");
                Console.WriteLine("이제 전투를 시작할 수 있습니다.");
                Console.WriteLine("\n1. 상태보기\n2. 전투시작");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n0. 종료하기\n");
                Console.ResetColor();

                Console.WriteLine("원하시는 행동을 입력하세요.");
                Console.Write(">>> ");


                string action = Console.ReadLine();

                switch (action)
                {
                    case "0":
                        Console.WriteLine("게임을 종료합니다");
                        return;

                    case "1":
                        StatusScreen();
                        break;

                    case "2":
                        BattleScreen(character, monsters);
                        break;

                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        Thread.Sleep(1000); // 메세지 출력 후 1초 뒤에 넘어감
                        continue;
                }
            } while (true);
        }

        public static void StatusScreen()
        {
            Console.Clear();
            Console.WriteLine("상태창으로 이동합니다.");

            Console.WriteLine("0을 메뉴로 입력하면 돌아갑니다");
            string action = Console.ReadLine();
            
            if(action == "0")
            {
                return;

            }
        }

        public static void BattleScreen(Character character, List<Monster> monsters)
        {
            Console.Clear();
            Random random = new Random();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Battle!\n");
            Console.ResetColor();
            
            
            int monsterNum = random.Next(1, 5);
            for(int i = 0; i< monsterNum; i++)
            {
                int j = random.Next(0, 3);
                Console.WriteLine($"Lv.{monsters[j].level} {monsters[j].name} HP {monsters[j].hp}");
            }

            Console.WriteLine("\n\n");

            Console.WriteLine("[내정보]");
            Console.WriteLine($"LV. {character.level}  {character.name} ({character.job})\nHP {character.hp}/100");

            Console.WriteLine("\n1. 공격\n0. 돌아가기\n");
            
            
            Console.WriteLine("원하시는 행동을 입력하세요.");
            Console.Write(">>> ");
            string action = Console.ReadLine();
            
            if (action == "0")
            {
                return;
            }
            else if (action == "1")
            {
                BattlePhase(character, monsters);
            }

        }

        public static void BattlePhase(Character character, List<Monster> monsters)
        {
            Console.Clear();
            int num = 1;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Battle!\n");
            Console.ResetColor();

            foreach (Monster monster in monsters)
            {
                Console.WriteLine($"{num} Lv.{monster.level} {monster.name} HP {monster.hp}");
                num++;
            }
            Console.WriteLine("\n\n");

            Console.WriteLine("[내정보]");
            Console.WriteLine($"LV. {character.level}  {character.name} ({character.job})\nHP {character.hp}/100");

            Console.WriteLine("\n0. 취소\n");

            Console.WriteLine("대상을 선택해주세요.");
            Console.WriteLine(">>> ");
            string enemy = Console.ReadLine();


        }
    }
}
