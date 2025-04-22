using System.Reflection.Emit;
using System.Xml.Linq;

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
            static public void Damage(Unit attacker, Unit target, int skillInfo)
            {
                int[] choices = { 0 };
                Random random = new Random();
                //데미지 오차 10%
                int damageRate = (int)Math.Round(attacker.Att * 0.1);
                int damage = random.Next(attacker.Att - damageRate, attacker.Att + damageRate);
                int prevHP = target.HP;
                float skillRate;
                if (skillInfo == 99)
                {
                    skillRate = 1f;
                }
                skillRate = SkillList[skillInfo].DamageRate;
                int totalDamage = (int)Math.Round(damage * skillRate);
                target.HP -= totalDamage;

                

                int damageChance = random.Next(0, 101);

                Console.WriteLine("Battle!!");
                Console.WriteLine();
                Console.WriteLine($"{attacker.Name} 의 공격!");

                if (damageChance > 15)
                {
                    totalDamage = totalDamage + (int)Math.Round(totalDamage * 1.6f);
                    target.HP -= totalDamage;
                    Console.WriteLine($"{target.Name} 을(를) 맞췄습니다. [데미지 : {totalDamage}] - 치명타 공격!!");

                }
                else if (damageChance <= 90)
                {
                    totalDamage = 0;
                    Console.WriteLine($"{target.Name} 을(를) 공격했지만 아무일도 일어나지 않았습니다.");
                }
                else
                {
                    Console.WriteLine($"{target.Name} 을(를) 맞췄습니다. [데미지 : {damage}]");
                    target.HP -= totalDamage;
                }
                Console.WriteLine();
                Console.WriteLine($"Lv.{target.Level} {target.Name}");
                Console.WriteLine($"HP {prevHP} -> {(target.IsDead ? "Dead" : target.HP)}");
                Console.WriteLine();
                Console.WriteLine("0. 다음");
                int choice = Input(choices);
            }
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

        class Item
        {
            public int Price { get; set; }
            public string ItemName { get; set; }
            public string ItemToolTip { get; set; }
        }

        class Weapon : Item
        {
            public int WeaponAtt { get; set; }

            public Weapon(string name, string toolTip, int att, int price)
            {
                ItemName = name;
                ItemToolTip = toolTip;
                WeaponAtt = att;
                Price = price;
            }

        }

        class Armor : Item
        {
            public int ArmorDfn { get; set; }

            public Armor(string name, string toolTip, int dfn, int price)
            {
                ItemName = name;
                ItemToolTip = toolTip;
                ArmorDfn = dfn;
                Price = price;
            }
        }

        class Skill
        {
            public string Name { get; set; }
            public string ToolTip { get; set; }
            public float DamageRate { get; set; }
            public int HitChance { get; set; }
            public int MPCost { get; set; }
            public bool IsRandomTarget { get; set; }


            public Skill(string name, string toolTip, float damageRate, int hitChance, int mPCost, bool isRandomTarget)
            {
                Name = name;
                ToolTip = toolTip;
                DamageRate = damageRate;
                HitChance = hitChance;
                MPCost = mPCost;
                IsRandomTarget = isRandomTarget;
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

        static List<Item> ItemList = new List<Item>
        {
            new Weapon("나무검", "공격력+5 | 무기 | 근방에 자란 나무로 만든 무기 불에 금방 탈 것 같다.", 5, 1000),
            new Weapon("철 야구방망이", "공격력+50 | 무기 | 묘하게 세계관과 동떨어진 무기 그만큼 화력이 강해보인다.", 50, 25000),
            new Armor("나무 방패", "방어력+5 | 방어구 | 근방에서 자란 나무로 만든 방패 불에 약해보인다.", 5, 1000),
            new Armor("무쇠갑옷", "방어력+10 | 방어구 | 마을에서 조금 멀리 떨어진 곳에서 만든 갑옷 단단해보인다.", 10, 2000)
        };

        static List<Skill> SkillList = new List<Skill>
        {
            new Skill("알파 스트라이크", "공격력 * 2 로 하나의 적을 공격합니다.", 2.0f, 1, 10, false)
        };

        //플레이어 객체 생성
        static Player player;



        static void Main(string[] args)
        {
            player = new Player();
            CreatPlayer();
            MainMenu();
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
            int[] choices = { 0, 1, 2, 3 };

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
                    Inventory();
                    break;
                case 3:
                    DungeonSelect();
                    break;
            }
        }

        static void StatusScreen()
        {
            int[] choicese = { 0 };

            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            Console.WriteLine();
            Console.WriteLine($"LV. {player.Level:D2}");
            Console.WriteLine($"이름 : {player.Name}  ( {player.Job} )");
            Console.WriteLine($"공격력 : {player.Att}");
            Console.WriteLine($"방어력 : {player.Dfn}");
            Console.WriteLine($"체력 : {player.HP}/{player.MaxHP}");
            Console.WriteLine($"Gold : {player.Gold} G");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");

            int choice = Input(choicese);

            switch (choice)
            {
                case 0:
                    MainMenu();
                    break;
            }
        }

        static void Inventory()
        {
            int[] choices = { 0, 1 };

            Console.WriteLine("[ 인벤토리 ]");
            Console.WriteLine("보유 중인 아이템을 관리합니다.");
            Console.WriteLine();
            Console.WriteLine("[ 아이템 목록 ]");
            Console.WriteLine();
            Console.WriteLine(" 원하시는 행동을 입력해주세요.");
            Console.WriteLine("1. 장착 관리");
            Console.WriteLine("0. 나가기");

            int choice = Input(choices);

            switch (choice)
            {
                case 0:
                    MainMenu();
                    break;
                case 1:
                    Equip();
                    break;
            }
        }

        static void Equip()
        {

        }

        static void DungeonSelect()
        {

        }

        //던전 메서드
        //기본 정보(몬스터 정보 및 캐릭터 정보) 출력 후 선택지 제공
        //공격하면 전투 메서드 실행
        static void Encounter(int stageNum)
        {
            int[] choices = { 0, 1, 2 };

            Console.WriteLine($"Battle!!");
            Console.WriteLine();
            for (int i = 0; i < stage[stageNum].Count; i++)
            {
                Unit monster = stage[stageNum][i];
                Console.WriteLine($"LV. {monster.Level} {monster.Name} HP {(monster.IsDead ? "Dead" : monster.HP)}");
            }
            Console.WriteLine();
            Console.WriteLine("[내정보]");
            Console.WriteLine($"LV.{player.Level} {player.Name} ({player.Name})");
            Console.WriteLine($"HP {player.HP}/{player.MaxHP}");
            Console.WriteLine();
            Console.WriteLine("1. 공격");
            Console.WriteLine("2. 스킬");
            Console.WriteLine("0. 도망가기");
            Console.WriteLine();
            int choice = Input(choices);

            switch (choice)
            {
                case 0:
                    Console.WriteLine("도망쳤습니다.");
                    break;
                case 1:
                    SelectMonster(stageNum, 99);
                    break;
                case 2:
                    SkillMenu(stageNum);
                    break;
            }
        }

        static void SkillMenu(int stageNum)
        {
            int[] choices = Enumerable.Range(0, SkillList.Count).ToArray();

            Console.WriteLine($"Battle!!");
            Console.WriteLine();
            for (int i = 0; i < stage[stageNum].Count; i++)
            {
                Unit monster = stage[stageNum][i];
                Console.WriteLine($"LV. {monster.Level} {monster.Name} HP {(monster.IsDead ? "Dead" : monster.HP)}");
            }
            Console.WriteLine();
            Console.WriteLine("[내정보]");
            Console.WriteLine($"LV.{player.Level} {player.Name} ({player.Name})");
            Console.WriteLine($"HP {player.HP}/{player.MaxHP}");
            Console.WriteLine();
            for (int i = 0; i < SkillList.Count; i++)
            {
                Console.WriteLine($"{i + 1} {SkillList[i].Name} - MP {SkillList[i].MPCost}");
                Console.WriteLine($"{SkillList[i].ToolTip}");
            }
            Console.WriteLine("0. 취소");
            int choice = Input(choices);

            switch (choice)
            {
                case 0:
                    Encounter(stageNum);
                    break;
                default:
                    SelectMonster(stageNum, choice - 1);
                    break;
            }
        }

        //전투 메서드
        //스킬 99는 기본 공격
        static void SelectMonster(int stageNum, int skillInfo)
        {

            int monsterCount = stage[stageNum].Count;
            while (player.HP > 0 && monsterCount > 0)
            {
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

                int[] choices = choicesList.ToArray();


                if (SkillList[skillInfo].IsRandomTarget)
                {
                    for (int i = 0;i < SkillList[skillInfo].HitChance;i++)
                    {
                        Random random = new Random();
                        int x = random.Next(choicesList.Count);
                        int randomMonster = choicesList[x];
                        Battle(stageNum, randomMonster, skillInfo, ref monsterCount, SkillList[skillInfo].HitChance);
                    }
                }

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
                Battle(stageNum, choice - 1, skillInfo, ref monsterCount);
            }
            BattleResult(stageNum, monsterCount);
        }

        static void Battle(int stageNum, int choiceMonster, int skillInfo, ref int monsterCount, int hitCount = 1)
        {
            for (int i = 0; i < hitCount; i++)
            {
                Unit.Damage(player, stage[stageNum][choiceMonster], skillInfo);
            }

            if (stage[stageNum][choiceMonster].HP <= 0)
            {
                monsterCount--;
            }

            foreach (Unit monster in stage[stageNum])
            {
                if (!monster.IsDead)
                {
                    Unit.Damage(monster, player, 99);
                }
            }
        }

        static void BattleResult(int stageNum, int monsterCount)
        {
            int[] choices = { 0 };
            Console.WriteLine("Battle!! - Result");
            Console.WriteLine();
            if (monsterCount <= 0)
            {
                Console.WriteLine("Victory");
                Console.WriteLine();
                Console.WriteLine($"던전에서 몬스터 {stage[stageNum]}마리를 잡았습니다.");
                Console.WriteLine();
                Console.WriteLine($"Lv.{player.Level} {player.Name}");
                Console.WriteLine($"HP {player.MaxHP} -> {player.HP}");
                Console.WriteLine();
                Console.WriteLine("0. 다음");
            }
            if (player.HP <= 0)
            {
                Console.WriteLine("You Lose");
                Console.WriteLine();
                Console.WriteLine($"Lv.{player.Level} {player.Name}");
                Console.WriteLine($"HP {player.MaxHP} -> 0");
                Console.WriteLine();
                Console.WriteLine("0. 다음");
            }
            int choice = Input(choices);

            switch (choice)
            {
                case 0:
                    DungeonSelect();
                    break;
            }
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