let uuidv1 = require('uuid/v1');
let BattleReadyPackage = require('../../../web-socket/packages/out/battleReady');
let BattleStartPackage = require('../../../web-socket/packages/out/battleStart');
let RoundStartPackage = require('../../../web-socket/packages/out/roundStart');
let RoundActionPackage = require('../../../web-socket/packages/out/roundAction');
let BattleCompletePackage = require('../../../web-socket/packages/out/battleComplete');
let EnumLib = require('../../../../server/app/enumList');
let DamageEvent = require('../actionsEvent/damage');
let MiniGameEvent = require('../actionsEvent/miniGame');
let MiddleActions = require('../actionsEvent/middleActions');
let DeadEvent = require('../actionsEvent/dead');
let MiniGameResultEvent = require('../actionsEvent/miniGameResult');

/**
 * Класс одного элемента боя
 * @type {Battle}
 */
module.exports = class Battle {

  constructor(app, options) {

    this.options = options;
    this.app = app;

    /**
     * Идентификатор боевки
     * @type {string}
     */
    this.uuid = uuidv1();
    /**
     * Дата начала боя
     * @type {date} Дата начала боя
     */
    this.timeStart = new Date();  // Время начала боя
    /**
     * Дата окончания боя
     * @type {Date}
     */
    this.timeEnd = new Date();  // Время начала боя
    /**
     * Клиенты участвующие в бою
     *
     * @type {Client[]}
     */
    this.clients = []; // Массив из участвующих мостров
    /**
     * Номер активного раунда
     * @type {number}
     */
    this.round = 0; // Активный раунд

    /**
     * Набор действий
     *
     * @type {object[]}
     */
    this.actions = [];

    /**
     * Ожидание действий от пользователей
     * @type {bool}
     */
    this.isReadyAction = false;

    /**
     * Бой закончен
     * @type {boolean}
     */
    this.isComplete = false;

    /**
     * Действие
     * @type {number}
     */
    this.actionNumber = -1;

    /**
     * Ожидание мини игры
     * @type {boolean}
     */
    this.isMiniGame = false;

    /**
     * Количество плееров законченых действие
     * @type {number}
     */
    this.playersActionComplete = 0;

    /**
     * Еффект чиха на сцене
     * @type {Array}
     */
    this.chihEffect = [];

  }

  /**
   * Добавление клиента
   * @param Client {object} Один из играков
   * @method
   */
  AddClient(client) {
    this.clients.push(client);
  }

  /**
   * Запуск боя
   * @method
   */
  Start() {
    this.actionNumber = -1;
    let sendMonsters = [];

    this.clients.forEach((val, ind, arr) => {

      let obj = {
        // uuid: val.monster.uuid,
        health: val.monster.maxHealth,
        // description: val.monster.description,
        monster: val.monster.monster,
        levelDescription: null,
        user: val.user.name,
        isBoat: val.boat,
        ammunitions: []
      };

      if (!val.boat) {

        let levelData = val.levelData.find(x => x.level === val.monster.monster.level);
        if (levelData != null)
          obj.levelDescription = levelData.description;
      }

      val.ammunitions.forEach((amm, ammInf, ammarr) => {
        obj.ammunitions.push({
          uuid: amm.uuid
        })
      });

      sendMonsters.push(obj);
    });

    // отсылаем пакет от предварительно готовности
    if (!this.clients[0].boat)
      this.app.wss.Send(new BattleReadyPackage(5, this.uuid, sendMonsters), this.clients[0].ws);
    if (!this.clients[1].boat)
      this.app.wss.Send(new BattleReadyPackage(5, this.uuid, sendMonsters), this.clients[1].ws);

    setTimeout(() => {

      // Отсылаем пакет о старте боя
      if (!this.clients[0].boat)
        this.app.wss.Send(new BattleStartPackage(this.uuid), this.clients[0].ws);
      if (!this.clients[1].boat)
        this.app.wss.Send(new BattleStartPackage(this.uuid), this.clients[1].ws);

      this.NextRound();

    }, this.options.secondBeforeStart * 1000);

  }

  /**
   * Поиск клиента по uuid монстру
   * @param monsterUuin
   * @return {*}
   * @method
   */
  FindClientByMonsterUuid(monsterUuid) {
    return this.clients.find(x => x.monster.monsterUuid === monsterUuid);
  }

  /**
   * Поиск клиента по токену
   * @param token
   * @return {null|Client} Клиент
   * @method
   */
  FindClientByToken(token) {
    return this.clients.find(x => x.token === token);
  }

  /**
   * Добавление события
   * @type {string} Токен
   * @type {number} Действие
   * @return {null}
   * @method
   */
  AddAction(token, data) {

    // Если действия уже не принимются, отказать
    if (!this.isReadyAction) return;
    if (data.round !== this.round) return;

    // Находим клиента
    let client = this.FindClientByToken(token);

    let action = this.FindActionByRound(data.round);

    let act = action.players.find(x => x.monster === client.monster.uuid);

    act.actions.push({
      step: data.step,
      action: data.action
    });

  }

  /**
   * Поиск активного раунда
   * @param roundNum
   * @return {null|object}
   * @constructor
   */
  FindActionByRound(roundNum) {
    return this.actions.find(x => x.round === roundNum);
  }

  /**
   * Запуск следующего раунда
   * @method
   */
  NextRound() {

    this.round++;

    this.actions.push(
      {
        round: this.round,
        players: [
          {
            monster: this.clients[0].monster.uuid,
            isBoat: this.clients[0].boat,
            actions: []
          },
          {
            monster: this.clients[1].monster.uuid,
            isBoat: this.clients[1].boat,
            actions: []
          }
        ],
        result: [],
        isComplete: false
      }
    );

    // Рассылаем уведомление о старте раунда
    this.clients.forEach((item, index, clients) => {
      if (!item.boat)
        this.app.wss.Send(new RoundStartPackage(this.round), item.ws);
    });

    this.isReadyAction = true;

    setTimeout(() => {
      this.ProcessBattleRound();
    }, this.options.secondActions * 1000)

  };

  /**
   * Обработка массива присланныхданных
   * @method
   */
  ProcessBattleRound() {

    this.isReadyAction = false;
    this.actionNumber = -1;

    if (this.clients.find(x => x.boat))
      this.CreateBoatActions();

    // Добавляются пустые действия на случай отсутствия действий
    let act = this.FindActionByRound(this.round);
    act.players.forEach((elem, index, arr) => {
      for (let i = 0; i < this.options.actionInRound; i++) {
        if (elem.actions.length < i + 1) {
          elem.actions.push({
            step: i + 1,
            action: 0
          });
        }
      }
    });

    // Обработка эффектов аммуниции
    this.clients.forEach((cl, clInd, clList) => {
      cl.ammunitions.forEach((am, amInd, amList) => {

        try {
          if (am.BeforeRound) {
            am.BeforeRound({
              battle: this,     // Бой
              parent: cl,       // Владелец предмета
              actions: act      // Действия
            });
          }
        } catch (e) {
          this.app.models.DebugMailer.SendAdminMail("Server error: process BeforeRound", e);
        }

      });
    });

    this.ProcessNextAction();
  }

  /**
   * Готовность пользователя к получению следующего действия
   * @param clientToken
   * @constructor
   */
  PlayerActionComplete(clientToken) {

    this.clients.find(x => x.token === clientToken).ready = true;

    if (this.clients.filter(x => x.boat || x.ready).length >= 2) {

      if (this.timerActionWait) {
        clearTimeout(this.timerActionWait);
      }

      this.ProcessNextAction();

      for (let i = 0 ; i < this.clients.length ; i++)
        this.clients[i].ready = false;
    }

  }

  // Готовность игрока миниигры
  PlayerMiniGameComplete(ws, data) {

    this.clients.find(x => x.token === ws.token).miniGame = data;

    let client = this.clients.find(x => x.boat);

    if(client != null){
      client.miniGame = {
        point: Math.floor(Math.random() * 4),
        //point: 2,
        time: 10
      }
    }

    let clientList = this.clients.filter(x => x.boat || x.miniGame != null);

    if (clientList.length >= 2) {
      this.MiniGameComplete();
    }

  }

  // Окончание миниигры
  MiniGameComplete(){

    if(this.timerActionWait)
      clearTimeout(this.timerActionWait);

    //let resultEvent = this.FindActionByRound(this.round).result.find(x => x.number === this.actionNumber).actions;
    let resultEvent = this.FindActionByRound(this.round).result.find(x => x.number === this.actionNumber).actions;

    let indexMiniGame = resultEvent.findIndex(x => x.eventId === 2);

    if (indexMiniGame >= 0)
      resultEvent.splice(indexMiniGame, 1);

    let resultMiniGame = [];
    this.clients.forEach((elem,ind,arr)=>{

      if(elem.miniGame === undefined || elem.miniGame === null){
        elem.miniGame = {
          point: 0,
          time: 0
        }
      }

      resultMiniGame.push({
        monster: elem.monster.uuid,
        miniGame: elem.miniGame
      })
    });

    resultEvent.push(new MiniGameResultEvent({data: resultMiniGame}));

    //let boat = this.clients.find(x => x.boat);

    // if (boat != null) {
    //   this.FindActionByRound(this.round).players.find(x => x.monster === boat.monster.uuid).actions[this.actionNumber].action = EnumLib.actionType.none;
    // } else {

      if (this.clients[0].miniGame.point > this.clients[1].miniGame.point) {
        this.FindActionByRound(this.round).players[1].actions[this.actionNumber].action = EnumLib.actionType.none;
      } else if (this.clients[0].miniGame.point < this.clients[1].miniGame.point) {
        this.FindActionByRound(this.round).players[0].actions[this.actionNumber].action = EnumLib.actionType.none;
      } else {
        // При ничьей бъют оба
        //this.FindActionByRound(this.round).players[0].actions[this.actionNumber].action = EnumLib.actionType.none;
        //this.FindActionByRound(this.round).players[1].actions[this.actionNumber].action = EnumLib.actionType.none;
      }

    // }
    this.clients[0].miniGame = null;
    this.clients[1].miniGame = null;

    this.ProcessNextAction(false);
  }

  /**
   * Обработка нового действия
   * @method
   */
  ProcessNextAction(changeActionNumber = true) {

    if (this.isComplete) return;

    let act = this.FindActionByRound(this.round);
    if (changeActionNumber) {
      this.actionNumber++;
      act.result.push(
        {
          number: this.actionNumber,
          actions: []
        });
    }

    if (this.actionNumber >= this.options.actionInRound) {
      return this.NextRound();
    }

    this.clients.forEach((cl, clInd, clArr) => {

      cl.ammunitions.forEach((am, amInd, amArr) => {
        try {
          am.BeforeAction({
            battle: this,         // Бой
            parent: cl, // Владелец предмета,
            //actions: act.result.find(x => x.number === this.actionNumber).actions
            actions: act,
            events: act.result.find(x => x.number === this.actionNumber).actions
          });
        } catch (e) {
          console.log(e);
          this.app.models.DebugMailer.SendAdminMail("Server error: process BeforeAction", e);
        }
      })
    });

    let playerAct1 = act.players[0];
    let playerAct2 = act.players[1];

    this.isMiniGame = false;

    if (act.players.filter(x => (x.actions[this.actionNumber].action === EnumLib.actionType.bodyBlock) || (x.actions[this.actionNumber].action === EnumLib.actionType.headBlock)).length >= 2) {
      // oба выставили защиту

      // Ничего пока не происходит

    } else if (((act.players.filter(x => x.actions[this.actionNumber].action === EnumLib.actionType.bodyHit).length >= 2)
      || act.players.filter(x => x.actions[this.actionNumber].action === EnumLib.actionType.headHit).length >= 2)
    && (act.result.find(x => x.number === this.actionNumber).actions.length <= 0
        || (act.result.find(x => x.number === this.actionNumber).actions.find(a=>a.eventId === 21) == null )))
     {
      // Оба поставили атаку

      this.isMiniGame = true;
      act.result.find(x => x.number === this.actionNumber).actions.push(new MiniGameEvent());

    } else {

      // Если атакует первый игрок
      if (playerAct1.actions[this.actionNumber].action === EnumLib.actionType.headHit || playerAct1.actions[this.actionNumber].action === EnumLib.actionType.bodyHit) {

        this.AttackEvent(act, this.clients[0],
          playerAct1.actions[this.actionNumber].action,
          this.clients[1],
          playerAct2.actions[this.actionNumber].action);

      }

      // Если атакует второй игрок
      if (playerAct2.actions[this.actionNumber].action === EnumLib.actionType.headHit || playerAct2.actions[this.actionNumber].action === EnumLib.actionType.bodyHit) {

        this.AttackEvent(act, this.clients[1],
          playerAct2.actions[this.actionNumber].action,
          this.clients[0],
          playerAct1.actions[this.actionNumber].action);
      }

    }

    // Действия в конце раунда
    this.clients[0].ammunitions.forEach((am, amInd, amList) => {
      try {
        am.AfterAction({
          battle: this,         // Бой
          parent: this.clients[0], // Владелец предмета
          actions: act,         // Действия,
          events: act.result.find(x => x.number === this.actionNumber).actions
        });
      } catch (e) {
        console.log(e);
        this.app.models.DebugMailer.SendAdminMail("Server error: process AfterAction", e);
      }
    });

    this.clients[1].ammunitions.forEach((am, amInd, amList) => {
      try {
        am.AfterAction({
          battle: this,         // Бой
          parent: this.clients[1], // Владелец предмета
          actions: act,         // Действия,
          events: act.result.find(x => x.number === this.actionNumber).actions
        });
      } catch (e) {
        console.log(e);
        this.app.models.DebugMailer.SendAdminMail("Server error: process AfterAction", e);
      }
    });

    this.clients.forEach((cl, ind, arr) => {
      if (cl.monster.IsDead) {
        act.result.find(x => x.number === this.actionNumber).actions.push(new DeadEvent({
          monsterUuid: cl.monster.uuid
        }));
        this.isComplete = true;
      }
    });

    let sendPackage = new RoundActionPackage({
      actions: [{
        monsterUuid: playerAct1.monster,
        action: playerAct1.actions[this.actionNumber]
      }, {
        monsterUuid: playerAct2.monster,
        action: playerAct2.actions[this.actionNumber]
      }],
      result: act.result.find(x => x.number === this.actionNumber).actions
    });

    if (!this.clients[0].boat)
      this.app.wss.Send(sendPackage, this.clients[0].ws);
    if (!this.clients[1].boat)
      this.app.wss.Send(sendPackage, this.clients[1].ws);

    if (this.isComplete) {
      this.BattleComplete();
      return
    }

    // Таймер ожидания результата
    if(this.isMiniGame){
      this.timerActionWait = setTimeout(()=>{ this.MiniGameComplete();},this.options.secondWaitMiniGame * 1000);
    }else{
      this.timerActionWait = setTimeout(()=>{ this.ProcessNextAction();}, this.options.secondWaitActions * 1000);
    }

  }

  /**
   * Обработка атаки и защиты
   * @param roundActions {object} массив с действиями
   * @param attackClient {object} Атакующий клиент
   * @param attackAction {object} Действие атакующего
   * @param protectClient {object} Защищающийся клиент
   * @param protectAction {object} Защищающееся действие
   * @method
   */
  AttackEvent(roundActions, attackClient, attackAction, protectClient, protectAction) {

    let act = roundActions.result.find(x => x.number === this.actionNumber).actions;

    let attack = {
      damage: 1,
      isBlock: ((attackAction === EnumLib.actionType.bodyHit && protectAction === EnumLib.actionType.bodyBlock)
        || (attackAction === EnumLib.actionType.headHit && protectAction === EnumLib.actionType.headBlock))
    };

    if (attackAction.damage)
      attack.damage = attackAction.damage;

    // Перед атакой
    attackClient.ammunitions.forEach((am, amInd, amList) => {
      try {
        am.BeforeAttack({
          battle: this,         // Бой
          parent: attackClient, // Владелец предмета
          actions: act,         // Действия
          attack: attackAction // Атака
        }, attack);
      } catch (e) {
        console.log(e);
        this.app.models.DebugMailer.SendAdminMail("Server error: process BeforeAttack", e);
      }
    });

    if (!attack.isBlock) {
      protectClient.ammunitions.forEach((am, amInd, amList) => {
        try {
          am.BeforeDamage({
            battle: this,           // Бой
            parent: protectClient,  // Владелец предмета
            actions: act,           // Действия
            attack: attackAction,   // Атакаfloor
            protect: protectAction // Защита
          }, attack);
        } catch (e) {
          console.log(e);
          this.app.models.DebugMailer.SendAdminMail("Server error: process BeforeAttack", e);
        }
      });
    }

    // Обработка после атаки
    attackClient.ammunitions.forEach((am, amInd, amList) => {
      try {
        am.AfterBeforeAttack({
          battle: this,         // Бой
          parent: attackClient, // Владелец предмета
          actions: act,         // Действия
          attack: attackAction // Атака
        }, attack);
      } catch (e) {
        console.log(e);
        this.app.models.DebugMailer.SendAdminMail("Server error: process AfterBeforeAttack", e);
      }

    });

    // Сама атака
    attack.damage = Math.max(0, attack.damage);
    if (!attack.isBlock) {
      protectClient.monster.SetDamage(attack.damage);
      act.push(new DamageEvent({
        monsterUuid: protectClient.monster.uuid,
        damage: attack.damage,
        live: protectClient.monster.actualHealth
      }));
    }

    // Пкет, обозначающий середину действия
    //act.push(new MiddleActions());

    // После атаки


    attackClient.ammunitions.forEach((am, amInd, amList) => {
      try {
        am.AfterAttack({
          battle: this,         // Бой
          parent: attackClient, // Владелец предмета
          actions: act,         // Действия
          attack: attackAction // Атака
        }, attack);
      } catch (e) {
        console.log(e);
        this.app.models.DebugMailer.SendAdminMail("Server error: process AfterAttack", e);
      }
    });

    if (!attack.isBlock) {
      protectClient.ammunitions.forEach((am, amInd, amList) => {
        try {
          am.AfterDamage({
            battle: this,           // Бой
            parent: protectClient,  // Владелец предмета
            actions: act,           // Действия
            attack: attackAction,   // Атака
            protect: protectAction  // Защита
          }, attack);
        } catch (e) {
          console.log(e);
          this.app.models.DebugMailer.SendAdminMail("Server error: process AfterDamage", e);
        }
      });
    }

  }

  /**
   * Создание случайных действий
   * @method
   */
  CreateBoatActions() {

    let action = this.FindActionByRound(this.round);
    let act = action.players.find(x => x.isBoat);

    for (let i = 0; i < this.options.actionInRound; i++) {

      act.actions.push({
        step: i,
        //action: Math.floor(Math.random() * (4 - 2 + 1)) + 2
        action: 3
      });
    }

  }

  /**
   * Раунд закончен со стороны всех клиентов
   * @param ws
   * @method
   */
  RoundClientComplete(ws) {

    //todo Добавить ограничение на окончание

    let objectActions = this.actions(act => act.round === this.round);
    let client = this.FindClientByToken(ws.token);

    let allComplete = true;

    for (let i = 0; i < objectActions.length; i++) {

      if (objectActions[i].monsterUuid = client.monster.monsterUuid) {
        objectActions[i].isComplete = true;
      }

      if (!objectActions[i].isComplete)
        allComplete = false;
    }

    let monsterDead = false;
    for (let i = 0; i < this.clients.length; i++) {
      if (this.clients[i].IsDead)
        monsterDead = true;
    }

    // Если какой либо из монстров помер, рассылаем информацию и сохраняем ой
    // if (monsterDead) {
    //   this.SaveBattleResult();
    //   //this.app.wss.Send();
    //   return;
    // }

    // Если пока никто не помер, стратуем новый монстр
    this.NextRound();
  }

  /**
   * Окончание боя
   * @method
   */
  BattleComplete() {

    let win = this.clients.find(x => !x.monster.IsDead);

    let winId = null;

    if (win != null)
      winId = win.monster.monster.id;

    // if (!this.clients[0].monster.IsDead)
    //   win = this.clients[0].monster.id;
    // else if(!this.clients[1].monster.IsDead)
    //   win = this.clients[1].monster.id;

    // Сохраняем бой
    this.app.models.battle.create({
        uuid: this.uuid,
        dateCreate: this.timeStart,
        roundCount: this.round,
        action: JSON.stringify(this.actions),
        winMonsterId: winId
      },
      (err, battleInst) => {

        if (err) {
          console.error(err);
          return
        }

        battleInst.monsters.add(this.clients[0].monster.id, function (err) {
          if (err)
            console.error(err)
        });

        battleInst.monsters.add(this.clients[1].monster.id, function (err) {
          if (err)
            console.error(err)
        });

        if (win == null) {
          this.SendResult({
            battleUuid: this.uuid
          });

          this.app.battle.RemoveBattle(this);
        } else {

          let winCount = this.GetWinPoint(win);

          if (win.monster.monster.wins == null)
            win.monster.monster.wins = 0;

          win.monster.monster.wins += winCount;
          win.monster.monster.save();

          this.SendResult({
            battleUuid: this.uuid,
            monsterUuid: win.monster.monster.uuid,
            winPoint: winCount,
            totalWinPoint: win.monster.monster.wins
          });

          this.app.battle.RemoveBattle(this);

        }
      }
    );

  }

  GetWinPoint(clientWin){

    let winCount = {
      winPoint: 1
    };

    let deadClient = this.clients.find(x => x.monster.IsDead);

    if(deadClient.monster.level < clientWin.monster.level)
      winCount.winPoint = 2;

    clientWin.ammunitions.forEach((am, amInd, amList) => {
      try {
        am.AfterBattle({
          battle: this,           // Бой
          parent: clientWin  // Владелец предмета
        },winCount);
      } catch (e) {
        this.app.models.DebugMailer.SendAdminMail("Server error: process AfterBattle", e);
      }
    });
  return winCount.winPoint;
  }

  SendResult(data) {

    let sendPackage = new BattleCompletePackage(data);

    if (!this.clients[0].boat)
      this.app.wss.Send(sendPackage, this.clients[0].ws);
    if (!this.clients[1].boat)
      this.app.wss.Send(sendPackage, this.clients[1].ws);
  }

  DisconnectUser(ws) {

    let client = this.clients.find(x => x.token === ws.token);
    client.monster.actualHealth = 0;
    this.BattleComplete();
  }

};
