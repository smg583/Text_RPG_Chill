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
            public int MaxHP { get; set; }
            public int MP { get; set; }
            public int MaxMP { get; set; }
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
            public int MaxMP { get; set; }
            public int StartGold { get; set; }

            public Job(string name, int level, int att, int dfn, int hp, int mp, int startGold)
            {
                Name = name;
                Level = level;
                Att = att;
                Dfn = dfn;
                MaxHP = hp;
                MaxMP = mp;
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

        class Quest
        {
            public string Name { get; set; }
            public string Info { get; set; } // 퀘스트 내용
            public string Condition { get; set; } // 퀘스트 달성 조건
            public int CompleteNum { get; set; } // 조건을 완료한 횟수 
            public int ConditionNum { get; set; } // 퀘스트 완료에 필요한 횟수 --- CompleteNUm >= ConditionNum이면 퀘스트 완료
            public int GoldReward { get; set; } // 골드 보상
            public bool isClear { get; set; } = false; // 퀘스트 클리어 여부
            public bool isAccept { get; set; } = false; // 퀘스트 수락 여부
            public bool isGetReward { get; set; } = false; // 보상 획득 여부 

            public Quest() { }

            public Quest(string questName, string questInfo, string questCondition, int completeNum, int conditionNum, int goldReward)
            {
                Name = questName;
                Info = questInfo;
                Condition = questCondition;
                CompleteNum = completeNum;
                ConditionNum = conditionNum;
                GoldReward = goldReward;
            }
        }

        //직업 리스트 -- 25/04/22 도적 및 팔라딘 추가 완료
        static List<Job> JobList = new List<Job>
{
    new Job("전사", 1, 10, 5, 100, 1500),
    new Job("도적", 1, 12, 3, 100, 1500),
    new Job("팔라딘", 1, 5, 10, 100, 1500)
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

        //퀘스트 리스트 - 퀘스트 이름, 내용, 달성 조건, 보상 순
        static List<Quest> questsList = new List<Quest>
        {
            new Quest("마을을 위협하는 미니언 처치", "이봐! 마을 근처에 미니언들이 너무 많아졌다고 생각하지 않나?\n마을주민들의 안전을 위해서라도 저것들 수를 좀 줄여야 한다고!\n모험가인 자네가 좀 처치해주게!",
                "미니언 5마리 처치", 0, 5, 50),
            new Quest("장비를 장착해보자", "던전 속 몬스터가 점점 강해지고 있다.\n이러다가는 내가 당하고 말거야..\n인벤토리 속 보유하고 있는 장비를 장착하여 강해진 몬스터들과 맞서보자!",
                "장비를 장착해보기", 0,  1, 100),
            new Quest("더욱 더 강해지기!", "몬스터를 처치하면 처치할 수록 강해지는 느낌이 든다..\n계속 강해지다 보면 용사의 검을 들어 올리지도..?",
                "LV.5 달성하기", 1, 5, 500)
        };

        //퀘스트 보상 아이템 리스트
        static List<Item> rewardItems = new List<Item>
        {
            new Shield("쓸만한 방패", "공격력 +1 | 방어력 +3 | 초보자가 쓰기에 그럭저럭 괜찮은 방패이다.", 1 , 3, 500),
            new Armor("가죽 갑옷", "방어력 + 5 | 마을에서 키우는 소 가죽으로 만든 갑옷이다. 질겨서 잘 안 찢어진다.", 5, 500),
            new Weapon("용사의 검","공격력 +20 | 마을의 동굴 최심부에 꽂혀있던 검. 용사의 검을 뽑은 자는 마왕을 물리칠 수 있는 힘을 얻게된다.", 20, 2000)
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
            player.MaxMP = choiceJob.MaxMP;
            player.MP = choiceJob.MaxMP;
            player.Gold = choiceJob.StartGold;
            player.Job = choiceJob.Name; // 버그픽스 : player.Job = choiceJob으로 실행하면 상태창에서 직업이 아닌 Pogram.Job이 출력 되어 player 클래스의 Job을 string으로 변환하여 Job.Name 할당
            player.PlayerEXP = 0;
        }

        //메인메뉴 메서드 -- 인벤토리 / 퀘스트 선택사항 추가
        static void MainMenu()
        {
            // 반복문으로 돌려  다른 메서드로 이동해도 메인은 계속 실행 되게함 -> 다른 메서드에서 메인메뉴로 돌아올 때 MainMenu()를 호출 하지 않고 return으로 돌아옴 -> 재귀호출을 계속하는 것을 방지
            while (true)
            {
                int[] choices = { 0, 1, 2, 3, 4 };

                Console.WriteLine("메인메뉴");
                Console.WriteLine("이제 전투를 시작할 수 있습니다.");
                Console.WriteLine();
                Console.WriteLine("1. 상태보기");
                Console.WriteLine("2. 인벤토리");
                Console.WriteLine("3. 던전입장");
                Console.WriteLine("4. 퀘스트 게시판");

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
                    case 4:
                        QuestsScreen();
                        break;
                }
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
            Console.WriteLine($"HP : {player.HP}/{player.MaxHP}");
            Console.WriteLine($"Gold : {player.Gold} G");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");

            int choice = Input(choicese);

            switch (choice)
            {
                case 0:
                    return;
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
                    return;
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
            Console.WriteLine($"MP {player.MP}/{player.MaxMP}");
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
            Console.WriteLine($"MP {player.MP}/{player.MaxMP}");
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
                    for (int i = 0; i < SkillList[skillInfo].HitChance; i++)
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
                Console.WriteLine($"MP {player.MP}/{player.MaxMP}");
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

        static void Damage(Unit attacker, Unit target, int skillInfo)
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

        // 퀘스트를 나타내는 스크린
        public static void QuestsScreen()
        {
            while (true)
            {
                int[] choises = { 0, 1, 2, 3 };
                int num = 1;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("QUEST!!\n");
                Console.ResetColor();
                Console.WriteLine("다양한 보상을 얻을 수 있는 퀘스트입니다.\n");


                foreach (Quest quest in questsList)
                {
                    if (quest.isAccept)
                    {
                        Console.WriteLine(num++ + ". " + quest.Name + " [진행중]");
                    }
                    else if (quest.isClear)
                    {
                        Console.WriteLine(num++ + ". " + quest.Name + " [완료]");
                    }
                    else
                    {
                        Console.WriteLine(num++ + ". " + quest.Name);
                    }

                }

                Console.WriteLine("\n");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("0. 뒤로가기\n");
                Console.ResetColor();

                int choice = Input(choises);

                switch (choice)
                {
                    case 0:
                        return;
                    case 1:
                        QuestAcceptScreen(questsList[0], rewardItems[0]);
                        break;
                    case 2:
                        QuestAcceptScreen(questsList[1], rewardItems[1]);
                        break;
                    case 3:
                        QuestAcceptScreen(questsList[2], rewardItems[2]);
                        break;
                }
            }


        }


        static void QuestAcceptScreen(Quest quest, Item rewardItem)
        {

            // 배열은 크기가 고정되기에 일단 리스트로 설정
            List<int> choicesList = new List<int>();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("QUEST!!\n");
            Console.ResetColor();

            //퀘스트 미진행, 진행, 완료에 따른 제목 변경
            if (quest.isClear)
            {
                Console.WriteLine($"{quest.Name} [완료]\n");
            }
            else if (quest.isAccept)
            {
                Console.WriteLine($"{quest.Name} [진행중]\n");
            }
            else
            {
                Console.WriteLine($"{quest.Name}\n");
            }

            Console.WriteLine($"{quest.Info}\n");
            Console.WriteLine($"{quest.Condition} ({quest.CompleteNum}/{quest.ConditionNum})\n");

            Console.WriteLine("- 보상 -");
            Console.WriteLine($"{rewardItem.ItemName}\n{quest.GoldReward} G\n");

            //퀘스트 미진행, 진행, 완료 및 보상 획득 여부에 따른 선텍지 변경 및 선택 번호를 리스트의 삽입           

            if (quest.isClear && quest.isGetReward == false)
            {
                Console.WriteLine("1. 보상받기\n0. 돌아가기");
                choicesList.Add(0);
                choicesList.Add(1);
            }
            else if (quest.isAccept || quest.isClear)
            {
                Console.WriteLine("0. 돌아가기");
                choicesList.Add(0);

            }
            else
            {
                Console.WriteLine("1. 수락\n2. 거절");
                choicesList.Add(1);
                choicesList.Add(2);
            }

            //Input에 넘기기 위해 리스트를 배열로 변환
            int[] choices = choicesList.ToArray();

            int choice = Input(choices);

            switch (choice)
            {
                case 0:
                    return;
                case 1:
                    if (quest.isClear && quest.isGetReward == false)
                    {
                        quest.isGetReward = true;
                        Console.WriteLine("보상을 획득하였습니다!");
                        inventory.Add(rewardItem);
                        player.Gold += quest.GoldReward;
                    }
                    else
                    {
                        quest.isAccept = true;
                        Console.WriteLine("퀘스트를 수락하였습니다.");
                    }
                    Thread.Sleep(1000);
                    Console.Clear();
                    return;
                case 2:
                    Console.WriteLine("퀘스트를 거절하였습니다.");
                    Thread.Sleep(1000);
                    Console.Clear();
                    return;
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