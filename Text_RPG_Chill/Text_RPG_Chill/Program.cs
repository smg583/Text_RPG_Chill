﻿using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace TextRPG_Team_ver
{
    internal class Program
    {
        //플레이어, 몬스터를 통합하는 유닛 클래스
        class Unit
        {
            public string Name { get; set; }
            public int Level { get; set; }
            public float Att { get; set; }
            public float Dfn { get; set; }
            public int HP { get; set; }
            public int MaxHP { get; set; }
            public int MP { get; set; }
            public int MaxMP { get; set; }
            public bool IsDead => HP <= 0;
        }
        //임시 플레이어 클래스
        class Player : Unit
        {
            public string Job { get; set; }
            public int Gold { get; set; }
            private int playerEXP { get; set; }
            public int RequireEXP { get; set; }

            public int PlayerEXP
            {
                get => playerEXP;
                set => playerEXP = value;
            }

            public void AddEXP(int amount)
            {
                playerEXP += amount;
                CheckLevelUp(); // setter 호출 없이 직접 처리
            }

            //요구사항 요구경혐치 10 - 35 - 65 - 100 - ...
            //에 따라 요구경험치 상승폭을 조절했습니다
            public void CheckLevelUp()
            {
                while (playerEXP >= RequireEXP)
                {
                    playerEXP -= RequireEXP;
                    Level++;
                    Att += 1;
                    Dfn += 0.5f;
                    HP = MaxHP;
                    MP = MaxMP;
                    RequireEXP = (5 * Level * Level + 35 * Level - 20) / 2;

                    Console.WriteLine($"{Name}이(가) 레벨업 했습니다! 현재 레벨: {Level}");
                }
            }
        }



        //몬스터 클래스
        class Monster : Unit
        {
            public int GiveExp { get; set; }
            //몬스터 생성
            public Monster(string mosterName, int monsterLV, float monsterAtt, int monsterHP, int giveExp)
            {
                Name = mosterName;
                Level = monsterLV;
                Att = monsterAtt;
                HP = monsterHP;
                GiveExp = giveExp;
            }

            //전투에 사용할 몬스터 클론
            public Monster Clone()
            {
                return new Monster(this.Name, this.Level, this.Att, this.HP, this.GiveExp);
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

        //아이템 클래스
        class Item
        {
            public int Price { get; set; }
            public string ItemName { get; set; }
            public string ItemToolTip { get; set; }

           
        }

        //아이템 - 무기
        class Weapon : Item
        {
            public float WeaponAtt { get; set; }

            public Weapon(string name, string toolTip, float att, int price)
            {
                ItemName = name;
                ItemToolTip = toolTip;
                WeaponAtt = att;
                Price = price;
            }

        }

        //아이템 - 방어구
        class Armor : Item
        {
            public float ArmorDfn { get; set; }

            public Armor(string name, string toolTip, float dfn, int price)
            {
                ItemName = name;
                ItemToolTip = toolTip;
                ArmorDfn = dfn;
                Price = price;
            }
        }

        //방패 아이템 타입 추가
        class Shield : Item
        {
            public float ShieldDfn { get; set; }
            public float ShieldAtt { get; set; }

            public Shield(string name, string toolTip, float att, float dfn, int price)
            {
                ItemName = name;
                ItemToolTip = toolTip;
                ShieldAtt = att;
                ShieldDfn = dfn;
                Price = price;
            }
        }

        class Potion : Item
        {
            public int Heal { get; set; }
            public string potionType;// 포션의 타입을 나누는 변수 체력은 Hp 마나는 Man
            public Potion(string name, string potionType, string toolTip, int heal, int price)
            {
                ItemName = name;
                ItemToolTip = toolTip;
                this.potionType = potionType;
                Heal = heal;
                Price = price;
            }
        }
        

        //스킬 클래스
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

        //퀘스트 클래스
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
            new Job("전사", 1, 10, 5, 100, 50, 1500),
            new Job("도적", 1, 12, 3, 100, 50, 1500),
            new Job("팔라딘", 1, 5, 10, 100, 50, 1500)
        };

        //스테이지 별 몬스터 리스트
        //요구사항 : 몬스터 레벨은 = 경험치에 따라 경험치량 수정
        static Dictionary<int, Monster> monsters = new Dictionary<int, Monster>
        {
            { 0, new Monster("미니언", 2, 5, 15, 2) },
            { 1, new Monster("대포미니언", 5, 8, 25, 5) },
            { 2, new Monster("공허충", 3, 9, 10, 3) },
            { 3, new Monster("공성미니언", 7, 10, 25, 7) }
        };

        //스테이지 리스트
        static List<List<int>> stage = new List<List<int>>
        {
            new List<int> { 0, 1, 2 },
            new List<int> { 0, 1, 3 }
        };

        static Dictionary<int, (int Gold, Item? RewardItem)> stageReward = new Dictionary<int, (int, Item?)>
        {
            {0, (100, null) },
            {1, (300, ItemList?[0]) },
        };

        static List<Item> ItemList = new List<Item>
        {
            new Weapon("나무검", "공격력+5 | 무기 | 근방에 자란 나무로 만든 무기 불에 금방 탈 것 같다.", 5, 100),
            new Weapon("철 야구방망이", "공격력+50 | 무기 | 묘하게 세계관과 동떨어진 무기 그만큼 화력이 강해보인다.", 50, 500),
            new Armor("나무 방패", "방어력+5 | 방어구 | 근방에서 자란 나무로 만든 방패 불에 약해보인다.", 5, 80),
            new Armor("무쇠갑옷", "방어력+10 | 방어구 | 마을에서 조금 멀리 떨어진 곳에서 만든 갑옷 단단해보인다.", 10, 200),
            new Potion("빨간 포션","Hp", "체력+30 | 포션 | 유리병에 담긴 새빨간 포션.", 30, 30),
            new Potion("파란 포션","Mana", "마나 +50 | 포션 | 유리병에 담긴 파란 포션.", 50, 50)
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
            new Skill("알파 스트라이크", "공격력 * 2 로 하나의 적을 공격합니다.", 2.0f, 1, 10, false),
            new Skill("더블 스트라이크", "공격력 * 1.5 로 2명의 적을 랜덤으로 공겨합니다.", 1.5f, 2, 15, true)
        };

        static List<List<Monster>> combatStage = new List<List<Monster>>();


        //플레이어 객체 생성
        static Player player;
        static Random random = new Random();



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
            player.RequireEXP = 10;
        }

        //메인메뉴 메서드 -- 인벤토리 / 퀘스트 선택사항 추가
        static void MainMenu()
        {
            // 반복문으로 돌려  다른 메서드로 이동해도 메인은 계속 실행 되게함 -> 다른 메서드에서 메인메뉴로 돌아올 때 MainMenu()를 호출 하지 않고 return으로 돌아옴 -> 재귀호출을 계속하는 것을 방지
            while (true)
            {
                QuestClearAlarm();

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
                        // 플레이어 체력 또는 마나가 최대 보다 적을 시 회복화면으로 이동
                        if(player.HP < player.MaxHP || player.MP < player.MaxMP)
                        {
                            HealScreen();
                        }
                        else DungeonSelect();
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

            Console.WriteLine("[상태 보기]");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            Console.WriteLine();
            Console.WriteLine($"LV. {player.Level:D2}");
            Console.WriteLine($"이름 : {player.Name}  ( {player.Job} )");
            Console.WriteLine($"공격력 : {player.Att}");
            Console.WriteLine($"방어력 : {player.Dfn}");
            Console.WriteLine($"HP : {player.HP}/{player.MaxHP}");
            Console.WriteLine($"MP : {player.MP}/{player.MaxMP}");
            Console.WriteLine($"Gold : {player.Gold} G");
            Console.WriteLine($"EXP : {player.PlayerEXP}/{player.RequireEXP}");
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
            foreach (var item in ItemList)
                Console.WriteLine($"- {item.ItemName}: {item.ItemToolTip}");
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

        // Fix for CS0446: Replace 'Inventory' with 'ItemList' as 'Inventory' is not defined as a collection.
        static void Equip()
        {
            Console.Clear();
            Console.WriteLine("[ 장착 관리 ]\n 보유 중인 아이템을 장착합니다.\n");
            Console.WriteLine();

            Weapon equippedWeapon = null;
            Armor equippedArmor = null;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== [장비 목록] ====");

                int index = 1;
                Dictionary<int, Item> itemMap = new Dictionary<int, Item>();

                // Use 'ItemList' instead of 'Inventory' as 'Inventory' is not defined.
                foreach (var item in ItemList)
                {
                    itemMap[index] = item;

                    Console.Write($"{index}. ");

                    // 무기 표시
                    if (item is Weapon weapon)
                    {
                        if (equippedWeapon == weapon)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("[E] ");
                            Console.ResetColor();
                        }
                        Console.WriteLine($"{weapon.ItemName} (공격력 +{weapon.WeaponAtt})");
                    }

                    // 방어구 표시
                    else if (item is Armor armor)
                    {
                        if (equippedArmor == armor)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("[E] ");
                            Console.ResetColor();
                        }
                        Console.WriteLine($"{armor.ItemName} (방어력 +{armor.ArmorDfn})");
                    }

                    index++;
                }

                Console.WriteLine("\n번호를 입력해 장비를 장착/해제하세요.");
                Console.WriteLine("0. 나가기");
                Console.Write("\n>> ");

                string input = Console.ReadLine();
                if (input == "0")
                {
                    MainMenu();
                    break;
                }

                if (int.TryParse(input, out int choice) && itemMap.ContainsKey(choice))
                {
                    var selectedItem = itemMap[choice];

                    if (selectedItem is Weapon weapon)
                    {
                        if (equippedWeapon == weapon)
                        {
                            // 해제
                            equippedWeapon = null;
                            player.Att -= weapon.WeaponAtt;
                            Console.WriteLine($"{weapon.ItemName}을(를) 해제했습니다. (공격력 -{weapon.WeaponAtt})");
                        }
                        else
                        {
                            // 기존 장비 해제
                            if (equippedWeapon != null)
                            {
                                player.Att -= equippedWeapon.WeaponAtt;
                            }

                            equippedWeapon = weapon;
                            player.Att += weapon.WeaponAtt;
                            Console.WriteLine($"{weapon.ItemName}을(를) 장착했습니다. (공격력 +{weapon.WeaponAtt})");
                        }
                    }
                    else if (selectedItem is Armor armor)
                    {
                        if (equippedArmor == armor)
                        {
                            // 해제
                            equippedArmor = null;
                            player.Dfn -= armor.ArmorDfn;
                            Console.WriteLine($"{armor.ItemName}을(를) 해제했습니다. (방어력 -{armor.ArmorDfn})");
                        }
                        else
                        {
                            // 기존 장비 해제
                            if (equippedArmor != null)
                            {
                                player.Dfn -= equippedArmor.ArmorDfn;
                            }

                            equippedArmor = armor;
                            player.Dfn += armor.ArmorDfn;
                            Console.WriteLine($"{armor.ItemName}을(를) 장착했습니다. (방어력 +{armor.ArmorDfn})");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                }

                Console.WriteLine("\n계속하려면 Enter를 누르세요...");
                Console.ReadLine();
            }
        }

        // 체력/마나 회복 메서드
        static void HealScreen()
        {
            while (true)
            {
                int[] choices = [0, 1, 2];

                // 아이템 리스트에서 Potion 클래스를 확인하고, 포션 타입이 Hp인지 Mana인지 확인
                int hpPotionNum = 0;
                int manaPotionNum = 0;
                foreach (Item item in ItemList)
                {
                    if (item is Potion) // 아이템의 클래스가 Potion일 경우
                    {
                        Potion potion = (Potion)item;  // item을 Potion으로 다운캐스팅 => .potionType을 사용하기 위해
                        // 포션 타입을 확인하여 포션 개수를 알아냄
                        if (potion.potionType == "Hp")
                        {
                            hpPotionNum++;
                        }
                        else if (potion.potionType == "Mana")
                        {
                            manaPotionNum++;
                        }
                    }
                }

                Console.WriteLine("던전에 들어가기 전에 회복을 할 수 있습니다.\n");
                Console.WriteLine("현재 포션 보유 수량");
                Console.WriteLine($"체력 포션: {hpPotionNum}개\n마나 포션: {manaPotionNum}개\n");

                Console.WriteLine("사용하실 포션을 선택하세요.");
                Console.WriteLine("1. 체력 포션\n2. 마나 포션\n0. 건너뛰기\n");

                int choice = Input(choices);

                switch (choice)
                {
                    case 0:
                        DungeonSelect();
                        return; // break가 아닌 return으로 메서드 종료
                    case 1:
                        if (hpPotionNum > 0)
                        {
                            foreach (Item item in ItemList)
                            {
                                if (item is Potion)
                                {
                                    Potion potion = (Potion)item;
                                    if (potion.potionType == "Hp")
                                    {
                                        player.HP += potion.Heal; // 포션의 회복량을 player의 현재 hp에 더함
                                        Console.WriteLine("체력이 회복 되었습니다!");
                                        ItemList.Remove(item);// 포션을 사용했으니 인벤토링에서 삭제
                                        break;
                                    }
                                }

                            }
                        }
                        else
                        {
                            Console.WriteLine("체력 포션이 없습니다!");
                        }
                        Thread.Sleep(1000);
                        Console.Clear();
                        break;
                    case 2:
                        if (manaPotionNum > 0)
                        {
                            foreach (Item item in ItemList)
                            {
                                if (item is Potion)
                                {
                                    Potion potion = (Potion)item;
                                    if (potion.potionType == "Mana")
                                    {
                                        player.MP += potion.Heal; // 포션의 회복량을 player의 현재 mp에 더함
                                        Console.WriteLine("마나가 회복 되었습니다!");
                                        ItemList.Remove(item);// 포션을 사용했으니 인벤토링에서 삭제
                                        break;
                                    }
                                }

                            }
                        }
                        else
                        {
                            Console.WriteLine("마나 포션이 없습니다!");
                        }
                        Thread.Sleep(1000);
                        Console.Clear();
                        break;

                }
            }           
        }

        static void DungeonSelect()
        {
            while (true)
            {
                //던전 생성 - 1부터 던전의 수만큼 정렬 후 0을 추가
                //0은 메인메뉴로 돌아가기
                //1 - ...는 던전입장
                int[] choices = Enumerable.Range(1, stage.Count).Append(0).ToArray();
                int i;
                Console.WriteLine("[던전]");
                Console.WriteLine("입장할 던전을 선택해주세요.");
                Console.WriteLine();
                for (i = 0; i < stage.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {i + 1}층");
                }
                Console.WriteLine($"0. 취소");
                int choice = Input(choices);

                switch (choice)
                {
                    case 0:
                        return;
                    default:
                        //선택된 스테이지의 몬스터를 클론화 하는 로직
                        combatStage = stage.Select(list => list.Select(id => monsters[id].Clone()).ToList()).ToList();
                        Encounter(choice - 1);
                        break;
                }
            }
        }

        //던전 메서드
        //기본 정보(몬스터 정보 및 캐릭터 정보) 출력 후 선택지 제공
        //공격하면 전투 메서드 실행
        static void Encounter(int stageNum)
        {
            int[] choices = { 0, 1, 2 };

            while (true)
            {
                //초기 사망 및 클리어 판정 확인
                if (!combatStage[stageNum].Any(mon => !mon.IsDead) || player.HP <= 0)
                {
                    BattleResult(stageNum);
                    return;
                }

                Console.WriteLine($"Battle!!");
                Console.WriteLine();
                for (int i = 0; i < combatStage[stageNum].Count; i++)
                {
                    Unit monster = combatStage[stageNum][i];
                    Console.WriteLine($"LV. {monster.Level} {monster.Name} HP {(monster.IsDead ? "Dead" : monster.HP)}");
                }
                Console.WriteLine();
                Console.WriteLine("[내정보]");
                Console.WriteLine($"LV.{player.Level} {player.Name} ({player.Job})");
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
                        return;
                    case 1:
                        //셀렉트 몬스터에선 매개함수로 스테이지 정보와 사용할 스킬 정보를 요구
                        //1번 선택지는 기본공격으로 스킬 정보가 없음
                        //99로 대체하여 스킬 정보 대체
                        SelectMonster(stageNum, 99);
                        break;
                    case 2:
                        SkillMenu(stageNum);
                        break;
                }
            }
        }

        //스킬 선택 메서드
        static void SkillMenu(int stageNum)
        {
            List<int> choicesList = new List<int>();
            int index = 1;

            //플레이어의 mp가 mpcost보다 많으면 선택지로 추가
            foreach (Skill skill in SkillList)
            {
                if (player.MP >= skill.MPCost)
                {
                    choicesList.Add(index);
                }
                index++;
            }

            // 스킬의 번호만 choicesList에  추가되어 0을 누르면 뒤로가기가 안되어 0을 직접 추가 하였습니다.
            choicesList.Add(0);
            int[] choices = choicesList.ToArray();

            Console.WriteLine($"Battle!!");
            Console.WriteLine();
            for (int i = 0; i < stage[stageNum].Count; i++)
            {
                Unit monster = combatStage[stageNum][i];
                Console.WriteLine($"LV. {monster.Level} {monster.Name} HP {(monster.IsDead ? "Dead" : monster.HP)}");
            }
            Console.WriteLine();
            Console.WriteLine("[내정보]");
            Console.WriteLine($"LV.{player.Level} {player.Name} ({player.Job})");
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
                    return;
                default:
                    SelectMonster(stageNum, choice - 1);
                    break;
            }
        }

        //전투 메서드
        //스킬 99는 기본 공격
        static void SelectMonster(int stageNum, int skillInfo)
        {
            while (player.HP > 0 && combatStage[stageNum].Any(mon => !mon.IsDead))
            {
                List<int> choicesList = new List<int>();
                int index = 1;

                //죽은 몬스터만을 선택지로 추가
                foreach (Unit monster in combatStage[stageNum])
                {
                    if (!monster.IsDead)
                    {
                        choicesList.Add(index);
                    }
                    index++;
                }

                //랜덤한 타겟을 가지는 스킬을 사용했을 경우
                if (skillInfo != 99 && SkillList[skillInfo].IsRandomTarget)
                {
                    //적을 타겟수만큼 랜덤으로 고르는 코드
                    List<int> randomTargetList = choicesList
                        .OrderBy(x => random.Next())
                        .Take(SkillList[skillInfo].HitChance)
                        .Select(x => x - 1)
                        .ToList();
                    Battle(stageNum, randomTargetList, skillInfo);
                    break;
                }

                choicesList.Add(0);
                int[] choices = choicesList.ToArray();

                Console.WriteLine($"Battle!!");
                Console.WriteLine();
                int y = 1;
                foreach (Unit monster in combatStage[stageNum])
                {
                    Console.WriteLine($"[{(monster.IsDead ? "-" : y)}] LV. {monster.Level} {monster.Name} HP {(monster.IsDead ? "Dead" : monster.HP)}");
                    y++;
                }
                Console.WriteLine();
                Console.WriteLine("[내정보]");
                Console.WriteLine($"LV.{player.Level} {player.Name} ({player.Job})");
                Console.WriteLine($"HP {player.HP}/{player.MaxHP}");
                Console.WriteLine($"MP {player.MP}/{player.MaxMP}");
                Console.WriteLine();
                Console.WriteLine("0. 취소");

                int choice = Input(choices);
                int targetMonster = choice - 1;

                List<int> targetList = new List<int>
                {
                    targetMonster
                };

                if (choice == 0)
                {
                    return;
                }
                else
                {
                    Battle(stageNum, targetList, skillInfo);
                    break;
                }
            }
        }

        //전투 메서드
        static void Battle(int stageNum, List<int> choiceMonster, int skillInfo)
        {
            int hitCount = 1;

            //mp소모
            if (skillInfo != 99)
            {
                player.MP -= SkillList[skillInfo].MPCost;
                hitCount = SkillList[skillInfo].HitChance;
            }

            //히트카운트(랜덤한 다수의 적을 때릴 때 다수의 수)보다 적이 적을 때
            //히트카운트 조절
            if (choiceMonster.Count < hitCount)
            {
                hitCount = choiceMonster.Count;
            }

            //히트카운트만큼 플레이어가 몬스터에게 데미지
            for (int i = 0; i < hitCount; i++)
            {
                int targetIndex = choiceMonster[i];
                Damage(player, combatStage[stageNum][targetIndex], skillInfo);
            }

            //살아남은 몬스터가 플레이어에게 데미지
            foreach (Unit monster in combatStage[stageNum])
            {
                if (!monster.IsDead)
                {
                    Damage(monster, player, 99);
                }
            }

            //mp회복
            if (player.MP < player.MaxMP)
            {
                player.MP += 10;
                if (player.MP >= player.MaxMP)
                {
                    player.MP = player.MaxMP;
                }
            }
        }

        static void Damage(Unit attacker, Unit target, int skillInfo)
        {
            //데미지 조절용 필드
            int[] choices = { 0 };
            //데미지 오차 10%
            int damageRate = (int)Math.Round(attacker.Att * 0.1f);
            int damage = random.Next((int)attacker.Att - damageRate, (int)attacker.Att + damageRate);
            int prevHP = target.HP;
            float skillRate;

            //스킬정보 99 = 기본공격
            //기본공격 시 데미지는 1배
            //스킬 시 데미지 각 스킬에 따라 변동
            if (skillInfo == 99)
            {
                skillRate = 1f;
            }
            else
            {
                skillRate = SkillList[skillInfo].DamageRate;
            }
            int totalDamage = (int)Math.Round(damage * skillRate);

            //치명타, 회피용 랜덤 함수
            int damageChance = random.Next(0, 101);

            Console.WriteLine("Battle!!");
            Console.WriteLine();
            Console.WriteLine($"{attacker.Name} 의 공격!");

            //15%로 치명타
            //10%로 회피
            //나머지 일반 타격
            if (damageChance < 15)
            {
                totalDamage = totalDamage + (int)Math.Round(totalDamage * 1.6f);
                target.HP -= totalDamage;
                Console.WriteLine($"{target.Name} 을(를) 맞췄습니다. [데미지 : {totalDamage}] - 치명타 공격!!");

            }
            else if (damageChance < 25)
            {
                Console.WriteLine($"{target.Name} 을(를) 공격했지만 아무일도 일어나지 않았습니다.");
            }
            else
            {
                target.HP -= totalDamage;
                Console.WriteLine($"{target.Name} 을(를) 맞췄습니다. [데미지 : {totalDamage}]");
            }

            // 퀘스트가 수락됐고 미니언이 죽을 경우 진행도 카운트 증가
            if (questsList[0].isAccept && prevHP > 0 && target.IsDead && target.Name == "미니언" && questsList[0].isClear == false)
            {
                questsList[0].CompleteNum++;
            }

            Console.WriteLine();
            Console.WriteLine($"Lv.{target.Level} {target.Name}");
            Console.WriteLine($"HP {prevHP} -> {(target.IsDead ? "Dead" : target.HP)}");
            Console.WriteLine();
            Console.WriteLine("0. 다음");
            int choice = Input(choices);
        }

        //전투 결과 창 인카운터에서 체크
        static void BattleResult(int stageNum)
        {
            int[] choices = { 0 };
            Console.WriteLine("Battle!! - Result");
            Console.WriteLine();

            //승리 시
            if (!combatStage[stageNum].Any(mon => !mon.IsDead))
            {
                //클리어시 랜덤 확률로 아이템 획득
                //미구현
                int rewardItem = random.Next(101);
                //전투한 몬스터의 경험치를 합쳐 획득
                int totalExp = combatStage[stageNum].Sum(mon => ((Monster)mon).GiveExp);
                player.AddEXP(totalExp);
                player.Gold += stageReward[stageNum].Gold;
                //if (rewardItem > 95)
                //{
                //    getItem(stageReward[stageNum].RewardItem);
                //}

                Console.WriteLine("Victory");
                Console.WriteLine();
                Console.WriteLine($"던전에서 몬스터 {stage[stageNum].Count}마리를 잡았습니다.");
                Console.WriteLine($"획득 경험치 : {totalExp}");
                Console.WriteLine($"획득 골드 : {stageReward[stageNum].Gold}");
                Console.WriteLine();
                Console.WriteLine($"Lv.{player.Level} {player.Name}");
                Console.WriteLine($"HP {player.MaxHP} -> {player.HP}");
                Console.WriteLine();
                Console.WriteLine("0. 다음");
            }
            //패배 시
            if (player.HP <= 0)
            {
                Console.WriteLine("You Lose");
                Console.WriteLine();
                Console.WriteLine($"Lv.{player.Level} {player.Name}");
                Console.WriteLine($"HP {player.HP} -> 0");
                Console.WriteLine();
                Console.WriteLine("0. 다음");
            }

            // 3변째 퀘스트가 수락 되면 실행 questsList[2].isClear == false는 나중에 클리어가 되고 나서도 계속 실행되는 것을 방지
            if (questsList[2].isAccept && questsList[2].isClear == false)
            {
                questsList[2].CompleteNum = player.Level;
            }

            int choice = Input(choices);

            switch (choice)
            {
                case 0:
                    combatStage = stage.Select(list => list.Select(id => monsters[id].Clone()).ToList()).ToList(); // 던전 선택 전 초기화
                    return;
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
                Console.WriteLine("[QUEST]\n");
                Console.ResetColor();
                Console.WriteLine("다양한 보상을 얻을 수 있는 퀘스트입니다.\n");

                foreach (Quest quest in questsList)
                {

                    if (quest.isClear)
                    {
                        Console.WriteLine(num++ + ". " + quest.Name + " [완료]");
                    }
                    else if (quest.isAccept)
                    {
                        Console.WriteLine(num++ + ". " + quest.Name + " [진행중]");
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

            if (quest.isClear && quest.isGetReward == false) // 클리어 했지만 보상을 받지 않았을 경우
            {
                Console.WriteLine("1. 보상받기\n0. 돌아가기");
                choicesList.Add(0);
                choicesList.Add(1);
            }
            else if (quest.isAccept || quest.isClear) //  진행중 또는 완료 및 보상을 수령했을 경우
            {
                Console.WriteLine("0. 돌아가기");
                choicesList.Add(0);

            }
            else // 수락하지 않았을 경우
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
                    if (quest.isClear && quest.isGetReward == false) //퀘스 클리어했지만 보상을 받지 않았을 경우
                    {
                        quest.isGetReward = true; 
                        Console.WriteLine("보상을 획득하였습니다!");
                        player.Gold += quest.GoldReward;
                        ItemList.Add(rewardItem);
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

        //퀘스트 클리어 및 보상 수령을 하지 않을 경우 나오는 알리미
        static void QuestClearAlarm()
        {
            int questnum = 1;
            bool wrote = false; // 콘솔이 출력이 되었는가 확인하는 변수

            foreach (Quest quest in questsList)
            {
                if (quest.CompleteNum >= quest.ConditionNum && quest.isGetReward == false)
                {
                    quest.isClear = true;
                    Console.WriteLine($"{questnum}번째 퀘스트가 완료되었습니다! 보상을 수령하세요!");
                    wrote = true;
                }
                questnum++;
            }
            if (wrote) // 콘솔이 출력 되어야 1초동안 콘솔 문자를 보여줌
            {
                Thread.Sleep(1000);
                Console.Clear();
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