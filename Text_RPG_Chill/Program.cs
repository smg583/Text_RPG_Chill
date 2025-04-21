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
            public string Class { get; set; }
            public float PlayerEXP { get; set; }
        }
        //임시 플레이어 데이터
        static Player player = new Player
        {
            Name = "Chill",
            Class = "전사",
            Level = 1,
            Att = 10,
            Dfn = 5,
            MaxHP = 100,
            HP = 100
        };

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
                Console.WriteLine($"LV.{player.Level} {player.Name} ({player.Class})");
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

        static void Main(string[] args)
        {
            int stageNum = 1;
            Dungeon(stageNum);
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
