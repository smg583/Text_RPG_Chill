using System;
using System.Security.Principal;
using System.Windows;



    class Program
    {
        // 캐릭터 정보
        // 레벨 / 이름 / 직업 / 공격력 / 방어력 / 체력 / Gold
        
        private static int level;
        private static string name;
        private static string job;
        private static int atk;
        private static int def;
        private static int hp;
        private static int gold;

        static void Main(string[] args)
        {
            SetData();
            DisplayMainUI();
        }

        static void SetData()
        {
            level = 1;
            name = "Chad";
            job = "전사";
            atk = 10;
            def = 5;
            hp = 100;
            gold = 1500;
        }

        static void DisplayMainUI()
        {
            Console.Clear();
            Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.\n");
            Console.WriteLine("이제 전투를 시작하실 수 있습니다.\n");
            Console.WriteLine("1. 상태 보기\n");
            Console.WriteLine("2. 전투 시작\n");
            Console.WriteLine("원하시는 행동을 입력해주세요.\n");
            Console.WriteLine("\n >>");

            int result = CheckInput(1,2);

            switch (result)
            {
                case 1:
                    DisplayStatus();
                    break;
                /*case 2:
                    StartBattle();
                    break;*/
            }

            static void DisplayStatus()
            {
                Console.Clear();
                Console.WriteLine("1. 상태 보기\n");
                Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");

                Console.WriteLine($"LV. {level :D2}");
                Console.WriteLine($"이름 : {name} {{ {job} }}");
                Console.WriteLine($"공격력 : {atk}");
                Console.WriteLine($"방어력 : {def}");
                Console.WriteLine($"체력 : {hp}");
                Console.WriteLine($"Gold : {gold} G");
                Console.WriteLine();
                Console.WriteLine("0. 나가기\n");
                Console.WriteLine("원하시는 행동을 입력해주세요.\n");
                Console.WriteLine("\n >>");
                
                
                int result = CheckInput(0, 0);

                switch (result)
                {
                    case 0:
                        DisplayMainUI();
                        break;
                }
            }
        }



    }

