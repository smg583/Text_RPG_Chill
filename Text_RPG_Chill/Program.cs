namespace Text_RPG_Chill
{
    internal class Program
    {
        //플레이어, 몬스터를 통합하는 유닛 클래스
        class Unit
        {
            public string Name { get; set; }
            public int Level { get; set; }
            public int Att { get; set; }
            public int Dfn { get; set; }
            public int HP { get; set; }
            public int MaxHP { get; set; }
            public bool IsDead => HP <= 0;
        }
        //임시 플레이어 클래스
        class Player : Unit
        {
            public Job Job { get; set; }
            public int Gold { get; set; }
            public float PlayerEXP { get; set; }
        }

        //몬스터 클래스
        class Monster : Unit
        {
            //몬스터 생성
            public Monster(string mosterName, int monsterLV, int monsterAtt, int monsterHP)
            {
                Name = mosterName;
                Level = monsterLV;
                Att = monsterAtt;
                HP = monsterHP;
            }
        }

        //직업 클래스
        class Job
        {
            public string Name { get; set; }
            public int Level { get; set; }
            public int Att { get; set; }
            public int Dfn { get; set; }
            public int MaxHP { get; set; }
            public int StartGold { get; set; }

            public Job(string name, int level, int att, int dfn, int hp, int startGold)
            {
                Name = name;
                Level = level;
                Att = att;
                Dfn = dfn;
                MaxHP = hp;
                StartGold = startGold;
            }
        }

        //직업 리스트
        static List<Job> JobList = new List<Job>
        {
            new Job("전사", 1, 10, 5, 100, 1500)
        };

        //스테이지 별 몬스터 리스트
        static List<Monster> monsters1 = new List<Monster>
        {
            new Monster("미니언", 2, 5, 15),
            new Monster("대포미니언", 5, 8, 25),
            new Monster("공허충", 3, 9, 10)
        };

        static List<Monster> monsters2 = new List<Monster>
        {
            new Monster("미니언1", 2, 5, 15),
            new Monster("대포미니언1", 5, 8, 25),
            new Monster("공허충1", 3, 9, 10),
            new Monster("공성미니언", 7, 10, 25)
        };

        //스테이지 리스트
        static List<List<Monster>> stage = new List<List<Monster>>
        {
            monsters1,
            monsters2
        };

        static Player player;


        static void Main(string[] args)
        {
            player = new Player();
            CreatPlayer();
            MainMenu();
            int stageNum = 1;
            Dungeon(stageNum);
        }

        //캐릭터 생성 메서드
        static void CreatPlayer()
        {
            //직업 리스트 만큼 선택지 제공
            int[] choices = Enumerable.Range(1, JobList.Count).ToArray();

            //이름 입력 및 초기 화면
            Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.");
            Console.WriteLine();
            Console.WriteLine("원하시는 이름을 설정해 주세요: ");
            Console.Write(">>");
            player.Name = Console.ReadLine();
            Console.WriteLine();

            //직업 선택 부분
            Console.WriteLine("해당 캐릭터의 직업을 설정해 주세요.(숫자만 입력)");
            Console.WriteLine("1. 전사");
            Console.WriteLine("2. 도적");
            Console.WriteLine("3. 팔라딘");

            //위의 배열을 통해 선택지 선택
            int choice = Input(choices);
            Job choiceJob = JobList[choice - 1];

            //직업의 스탯에 맞게 플레이어 스탯 초기화
            player.Level = choiceJob.Level;
            player.Att = choiceJob.Att;
            player.Dfn = choiceJob.Dfn;
            player.MaxHP = choiceJob.MaxHP;
            player.HP = choiceJob.MaxHP;
            player.Gold = choiceJob.StartGold;
            player.Job = choiceJob;
            player.PlayerEXP = 0;
        }

        //메인메뉴 메서드
        static void MainMenu()
        {
            int[] choices = { 0, 1, 2 };

            Console.WriteLine("메인메뉴");
            Console.WriteLine("이제 전투를 시작할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("1. 상태보기");
            Console.WriteLine("2. 던전입장");

            //색상 변경
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. 종료하기");
            Console.ResetColor();
            int choice = Input(choices);

            switch (choice)
            {
                case 0:
                    Console.WriteLine("게임을 종료합니다");
                    return;

                case 1:
                    StatusScreen();
                    break;

                case 2:
                    DungeonSelect();
                    break;
            }
        }

        //던전 메서드
        //기본 정보(몬스터 정보 및 캐릭터 정보) 출력 후 선택지 제공
        //공격하면 전투 메서드 실행
        static void Dungeon(int stageNum)
        {
            int[] choices = { 0, 1 };

            Console.WriteLine($"Battle!!");
            Console.WriteLine();

            //스테이지[넘버]의 수만큼 반복
            for (int i = 0; i < stage[stageNum].Count; i++)
            {
                //스테이지[넘버]의 요소를 monster로 저장 및 출력
                var monster = stage[stageNum][i];
                Console.WriteLine($"LV. {monster.Level} {monster.Name} HP {(monster.IsDead ? "Dead" : monster.HP)}");
            }
            Console.WriteLine();
            Console.WriteLine("[내정보]");
            Console.WriteLine($"LV.{player.Level} {player.Name} ({player.Name})");
            Console.WriteLine($"HP {player.HP}/{player.MaxHP}");
            Console.WriteLine();
            Console.WriteLine("1. 공격");
            Console.WriteLine("0. 도망가기");
            Console.WriteLine();
            int choice = Input(choices);

            switch (choice)
            {
                case 0:
                    Console.WriteLine("도망쳤습니다.");
                    break;
                case 1:
                    Fight(stageNum);
                    break;
            }
        }

        //전투 메서드
        static void Fight(int stageNum)
        {
            int monsterCount = stage[stageNum].Count;
            while (player.HP > 0 || monsterCount > 0)
            {
                //살아있는 몬스터만 선택지로 나타내기 위한 리스트 셋업
                List<int> choicesList = new List<int>();
                int index = 1;

                foreach (Unit monster in stage[stageNum])
                {
                    if (!monster.IsDead)
                    {
                        choicesList.Add(index);
                    }
                    index++;
                }

                //위의 셋업으로 선택지 생성
                int[] choices = choicesList.ToArray();

                Console.WriteLine($"Battle!!");
                Console.WriteLine();
                for (int i = 0; i < stage[stageNum].Count; i++)
                {
                    Unit monster = stage[stageNum][i];
                    Console.WriteLine($"[{i + 1}] LV. {monster.Level} {monster.Name} HP {(monster.IsDead ? "Dead" : monster.HP)}");
                }
                Console.WriteLine();
                Console.WriteLine("[내정보]");
                Console.WriteLine($"LV.{player.Level} {player.Name} ({player.Job})");
                Console.WriteLine($"HP {player.HP}/{player.MaxHP}");
                Console.WriteLine();

                int choice = Input(choices);
                Fighting(stageNum, choice - 1, ref monsterCount);
            }
        }

        static void Fighting(int stageNum, int choiceMonster, ref int monsterCount)
        {
            Damage(player, stage[stageNum][choiceMonster]);

            if (stage[stageNum][choiceMonster].HP <= 0)
            {
                monsterCount--;
            }

            foreach (Unit monster in stage[stageNum])
            {
                if (!monster.IsDead)
                {
                    Damage(monster, player);
                }
            }
        }

        //데미지 메서드
        //공격자와 피격자를 매개변수로 받아
        //플레이어 몬스터 상관없이 적용
        static void Damage(Unit attacker, Unit target)
        {
            int[] choices = { 0 };
            Random random = new Random();
            //데미지 오차 10%
            int damageRate = (int)Math.Round(attacker.Att * 0.1);
            int damage = random.Next(attacker.Att - damageRate, attacker.Att + damageRate);
            int prevHP = target.HP;
            target.HP -= damage;

            Console.WriteLine("Battle!!");
            Console.WriteLine();
            Console.WriteLine($"{attacker.Name} 의 공격!");
            Console.WriteLine($"{target.Name} 을(를) 맞췄습니다. [데미지 : {damage}]");
            Console.WriteLine();
            Console.WriteLine($"Lv.{target.Level} {target.Name}");
            Console.WriteLine($"HP {prevHP} -> {target.HP}");
            Console.WriteLine();
            Console.WriteLine("0. 다음");
            int choice = Input(choices);
        }

        //인풋 확인 시스템
        static int Input(int[] choices)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");
                string? choice = Console.ReadLine();

                //선택지마다 있는 배열을 참조
                //배열외의 입력이면 재입력 요청
                if (int.TryParse(choice, out int number) && choices.Contains(number))
                {
                    Console.Clear();
                    return number;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("다시 입력해주세요.");
                }
            }
        }
    }
}
