using UnityEngine;
using System.Collections;

// Квесты
public enum Quest {
    /* Социальные  */
    invateFriends,                      // Пригласить друга                                                     
    overtakeFriend,                     // Обогнать друга                                                       
    shareGame,                          // Пошарить Игру                                                        
    contactFb,                          // Подключить фейсбук                                                   
    /* Объекты */
    getCoins,                           // Собрать X монет                                                      
    getWeapon,                          // Собрать Х оружия                                                     
    rotateRullett,                      // Вращать рулетку Х раз                                                
    useWeapon,                          // Использовать X раз оружие общее                                      
    useWeaponTrap,                      // Использовать Х раз копкан                                            
    useWeaponSabel,                     // Использовать Х раз сабли                                             
    useWeaponPistol,                    // Использовать Х раз пистолет                                          
    useWeaponBomb,                      // Использовать Х раз бомбы                                             
    useWeaponMolotov,                   // Использовать Х раз молотов                                           
    useWeaponBuckshot,                  // Использовать Х раз картечь                                           
    useWeaponType,                      // Использовать Х видов оружия в 1 забеге                               
    putOutTorch,                        // Потушить Х факелов                                                   
    crushSpiders,                       // Раздавить Х пауков                                                   
    runNoWeapon,                        // Пробежать Х метров без использования оружия                          
    useBoost,                           // Использовать Х раз буст                                              
    useBoostSpeed,                      // Использовать Х раз буст ускоренного забега                           
    useBoostSkate,                      // Использовать Х раз буст скейта                                       
    useBoostBarrel,                     // Использовать Х раз буст бочки                                        
    useBoostMillwell,                   // Использовать Х раз буст колесо мельницы                              
    useBoostShip,                       // Использовать Х раз буст корабля                                      
    runNoDamage,                        // Пробежать Х метров без получения урона
    jumpBreack,                         // Перепрыгнуть через овраг                                             
    jumpBarrier,                        // Перепрыгнуть через препядствие                                       
    jumpBarrierDouble,                  // Перепрыгнуть через 2 препядствия сразу                               
    jumpHendingStone,                   // Перепрыгнуть через висячий шар                                       
    jumpGhost,                          // Перепрыгнуть приведение                                              
    runNoBarrier,                       // Пробежать Х не задев ни одного препядствия                           
    doubleJumpPadStone,                 // Выполнить двойной прыжок прямонад камнем    
    enemyShotButMiss,                   // Враг ударил, но промахнулся                                          
    underSpear,                         // Увернуться от копья                                                  
    jumpBoomerang,                      // Перепрыгнуть бумеранг                                                
    killEnemy,                          // Убить врага                                                          
    killEnemyActec,                     // Убить ацтека                                                         
    killEnemyHeadlessZombie,            // Убить почти безголового зомби                                        
    killEnemyAztecSpear,                // Убить ацтека с копьем                                                
    killEnemyWarriorBoomerang,          // Убить ацтека с бумерангом                                            
    killEnemyWarriorGiant,              // Убить гиганта                                                        
    killEnemyFatZombie,                 // Убить толстого зомби                                                 
    killEnemyTrap,                      // Убить врага копканом                                                 
    killEnemySabel,                     // Убить врага саблей                                                   
    killEnemyPistol,                    // Убить врага пистолетом                                               
    killEnemyBomb,                      // Убить врага бомбой                                                   
    killEnemyMolotov,                   // Убить врага молотовым                                                
    killEnemyBuckshot,                  // Убить врага картечью                                                 
    killEnemyBreack,                    // Убить врага над пропастью                                            
    killEnemyJump,                      // Убить врага в прыжке                                                 
    jumpedEnemy,                        // Перепрыгнуть врага     
    noUseDoubleJump,                    // Не использовать двойной прыжок сколько то метров   
    runDistance,                        // Пробежать расстояние
    enemyToch,                          // Коснуться врага
    jumpOnStone,                        // Запрыгнуть на камень
    spearDefend,                        // Отбить копье
    use2weapon,                         // Использовать 2 вида оружия
    newMyRecords,                       // Побить свой рекорд
    fallBreack,                         // Упасть в яму
    deadOfGate,                         // Врезаться в ворота
    getMagnet,                          // Схватить магнит
    getHearth,                          // Получить чердце
    knockHeadStone,                     // Сбросить бревно
    jumped,                             // Прыгнуть
    stoneDanger,                        // Пролететь в опасной близости от камня
    weaponMiss,                         // Промахнуться оружием 
    use3weapon,                         // Использовать 3 вида оружия
    useMagnet,                          // Использовать магнит
    runNoKillSpider,                    // Бежать не убивая пауков
    breakToLive,                        // Выжать после падения в яму
    breackDown1100_1500,                // Упасть в яму на расстоянии
    deadDistance2200_2400,              // Умереть на расстоянии
    runDino,                            // Пробежать на дино
    runSpider                           // Пробежать на пауке
}

// Enemyes
public enum EnemyTypes {
    aztec = 0,                              // Ацтека
    headlessZombie = 1,                     // Безголовый зомби
    aztecSpear = 2,                         // Ацтек с копьем
    warriorBoomerang = 3,                   // Ацтек с бумерангом
    fatZombie = 4,                          // Толстый зомби
    warriorGiant = 5,                       // Гигант
    whoreSplinter = 6,                      // Шлюха сплинтер
    whoreBra = 7,                           // Шлюха с лифчиками
    whoreGemini = 8,                        // Близнецы шлюхи
    whorePoduska = 9,                       // Шлюха с подушками
    bigMama = 10,                            // Большая мама
    aztecForward = 11,                       // Ацтек генерируемый спереди
    none = 12                                // Пустой объект
}

// идентификация объекта
public enum identifiedObject {
    rollingStone,                                       // Лежачий камень
    rollingStoneSkeleton,                               // Лежачий камень со скелетом
    hangingStone,                                       // Висячий камень
    boomerangWeapon,                                    // Оружие бумеранг
    spearWeapon,                                        // Оружие копье
    underwearWeapon,                                    // Нижнее белье
    pillowWeapon                                        // Подушка
}

public enum barrierTages {
    RollingStone,                                       // Стоячий камень
}

/// <summary>
/// Типы генерируемых препядствей
/// </summary>
public enum barrierGenerateTypes {
    breack,                                             // Яма
    stone,                                              // Камень
    stoneDouble,                                        // Комбнация: Два камня подряд
    hending,                                            // Висячие бревно или зомби
    stoneAndHending,                                    // Комбинация: камень, следом висячий кслетел или бревно
    stoneSkelet,                                        // Скелет из земли
    stoneSkeletDouble,                                  // Комбинация из 2х скелетов подряд
    ghost,                                              // Один призрак
    ghostGroup,                                         // Комбинация из 1-4х скелетов
    boxes,                                              // Ящики
    barrels,                                            // Бочки
    puddle,                                             // Лужа
    stickyFloor                                         // Паутина
}




// Типы интерфейсов
public enum InterfaceTypes {
    mainMenu,
    gamePlay,
    pause,
    statistic,
    shop,
    question,
    dialog,
    credits,
    timer,
    console
}

// Типы интерфейсов
public enum WeaponTypes {
    none,
    trap,
    sabel,
    gun,
    bomb,
    molotov,
    ship,
    flowers,
    candy,
    chocolate,
    spear,
    bumerang,
    head,
    airArrow,
    pillow,
    underwear,
    fire,
    bullet,
    pirats,
    enemy,
    stone,
    gate,
    airStone,
    hendingBarrier,
    pet,
    sablePlayer
}

public enum panelTypes {
    main,
    pause,
    questionEndGame,
    statistic
}