let Battle = require('./lib/battle');
let ClientClass = require('./lib/client');
let Monster = require('./lib/monster');
let AmmunitionLibrary = require('./ammunition/ammunitionLibrary');
let BattleReadyTimeoutPackage = require('../../web-socket/packages/out/battleReadyTimeout');

module.exports = function (app, options) {
  app.battle = new BattleManager(app, options);
};

/**
 * Сонтроллер боевки
 * @class
 */
class BattleManager {

  /**
   * @constructor
   */
  constructor(app, options) {

    this.options = options;

    this.app = app;
    /**
     * Список активных боев
     * @type {Battle[]} Список боев
     */
    this.battles = [];
    /** Список авторизированных клиентов */
    this.clients = [];

    this.clientsBattleReady = [];

    this.app.models.MonsterParam.find({},(err,objectArray)=>{
      this.monsterParams = objectArray;
    })

  }

  /**
   * Авторизация новго клиента
   * @param {object} client Аторизируемый клиент
   * @method
   */
  AddClient(client) {
    this.clients.push(client);
    console.log(`Login new client. Clients count - ${this.clients.length}`);
  }

  /**
   * Выход клиента
   * @param ws {object} Клиент веб сокет
   * @method
   */
  RemoveClient(ws) {

    let num = null;

    for (let i = 0; i < this.clients.length; i++) {
      if (ws.token && this.clients[i].token === ws.token) {
        num = i;
      }
    }

    if (num != null) {
      this.clients.splice(num, 1);
      console.log(`Client logout. Clients count - ${this.clients.length}`);
    }

    this.RemoveReadyClient(ws.token);
    this.DisconnectPlayer(ws);
  }

  /**
   * Получаю клиента по токену
   * @param token {string} Авторизационный токен (токен сессии)
   * @return {Client|null} Клиент
   * @method
   */
  GetClientByToken(token) {

    for (let i = 0; i < this.clients.length; i++) {
      if (token && this.clients[i].token === token) {
        return this.clients[i];
      }
    }
    return null;
  }

  /**
   * Поиск боя по токену клиента
   * @param token {string} Токен клиента
   * @return {Battle} Активный бои
   * @method
   */
  GetBattleWishClientToken(token) {

    for (let i = 0; i < this.battles.length; i++) {
      if (this.battles[i].clients[0].token === token) {
        return this.battles[i];
      }
      if (this.battles[i].clients[1].token === token) {
        return this.battles[i];
      }
    }
    return null;
  }

  /**
   * Созбание боя
   * @param firstClient Клиент создающий бой
   * @param secondClient Клиент, который был на ожидании
   * @method
   */
  CreateBattle(firstClient, secondClient) {

    secondClient.CancelBattleWait();

    let deleteReady;
    for (let i = 0; i < this.clientsBattleReady.length; i++) {
      if (secondClient.token === this.clientsBattleReady[i].client.token)
        deleteReady = i;
    }

    if (deleteReady != null)
      this.clientsBattleReady.splice(deleteReady, 1);

    let battle = new Battle(this.app, this.options);
    battle.AddClient(firstClient);
    battle.AddClient(secondClient);
    battle.Start();
    this.battles.push(battle);

    console.log(`Active Battle ${this.battles.length}`);

  }

  /**
   * Удаление боя
   * @param battle
   * @method
   */
  RemoveBattle(battle){

    let indx = this.battles.findIndex(x=>x.uuid === battle.uuid);
    if(indx != null)
      this.battles.splice(indx,1);

    console.log(`Active Battle ${this.battles.length}`);
  }

  /**
   * Добавляем action
   * @type {string} token Токен
   * @type {object} data Данные действия
   * @method
   */
  AddAction(token, data) {

    // Находим бой
    let battle = this.GetBattleWishClientToken(token);
    if (!battle) return;

    battle.AddAction(token, data);

  }

  DisconnectPlayer(ws){

    let battle = this.GetBattleWishClientToken(ws.token);
    if (!battle) return;
    battle.DisconnectUser(ws);
    //this.RemoveBattle(battle);
  }

  BattleRoundComplete(ws) {

    let battle = this.GetBattleWishClientToken(data.token);
    if (!battle) return;
    battle.RoundClientComplete(ws);
  }

  /**
   * Добавление клиента в список ожидаемых
   * @param client {Client} Добавление клиента в список ожидаемых
   * @method
   */
  AddReadyClient(client, timeOutBoat) {

    let boatWait = false;

    if(timeOutBoat !== undefined && timeOutBoat !== null)
      boatWait = timeOutBoat;

    // Не нашли оппонента, добавляемся в очеред.
    this.app.battle.clientsBattleReady.push(
      {
        client: client,
        dateTime: new Date()
      }
    );

    if(timeOutBoat){
      client.timerBattleWait = setTimeout(()=> {
        this.StartBattleWishBoat(client);
        },this.options.secondWaitBattleWishBoat*1000,client);
    }else{
      client.timerBattleWait = setTimeout(()=> {
        this.BattleWaitTimeOut(client);
        },this.options.secondWaitBattle*1000,client);
    }

  }

  /**
   * Старт боя с ботом
   * @param client
   * @method
   */
  StartBattleWishBoat(client){

    client.CancelBattleWait();

    this.CreateBoat(client, (err, opponent)=>{
      this.CreateBattle(client, opponent);
    });
  }

  /**
   * Таймаут ожидания боя
   * @param client
   * @method
   */
  BattleWaitTimeOut(client){

    client.CancelBattleWait();

    this.app.wss.Send(new BattleReadyTimeoutPackage(), this.clients[0].ws);
    this.RemoveReadyClient(client.token);
  }

  /**
   * Удаление клиентов для ожидания
   * @param deleteToken {string} Токен для удаления
   * @method
   */
  RemoveReadyClient(deleteToken) {

    let deleteReady;
    for (let i = 0; i < this.clientsBattleReady.length; i++) {
      if (deleteToken === this.clientsBattleReady[i].client.token)
        deleteReady = i;
    }

    if (deleteReady != null)
      this.clientsBattleReady.splice(deleteReady, 1);
  }

  /**
   * Созбание бота
   * @method
   */
  CreateBoat(client, callback){

    let boat = new ClientClass();
    boat.boat = true;

    //boat.ammunitions.push(AmmunitionLibrary.GetRandomShield());
    //boat.ammunitions.push(AmmunitionLibrary.GetRandomHelmet());
    //boat.ammunitions.push(AmmunitionLibrary.GetRandomWeapon());
    //boat.ammunitions.push(AmmunitionLibrary.GetAmmunition('11ceaaa0-43f7-11e8-a030-8708785d77e2'));

    let readyLevels = [];

    switch (client.monster.monster.level){
      case 15:
        readyLevels.push(15);
        readyLevels.push(17);
        break;
      case 17:
        readyLevels.push(15);
        readyLevels.push(17);
        readyLevels.push(19);
        break;
      case 19:
        readyLevels.push(17);
        readyLevels.push(19);
        break;
    }
    let moblevel = readyLevels[Math.floor(Math.random() * readyLevels.length)];

    this.app.models.Monster.findOne({ where: {and: [ {userId: 17},{level: moblevel}]}},(err,model)=>{

      boat.monster = new Monster(model,this.app.battle.monsterParams.find(x=>x.level === model.level));
      boat.monster.health = 10;
      boat.monster.BattleInitiate();

      callback(err, boat);

    });
  }

  /**
   * Поиск соперника
   * @param monster
   * @return {*}
   * @return
   */
  FindOpponent(monster) {

    if (this.clientsBattleReady.length <= 0) {
      return null;
    }
    let clientIndex = this.clientsBattleReady.findIndex(x => x.client.monster.monster.level >= monster.monster.level - 2
      && x.client.monster.monster.level <= monster.monster.level + 2);

    let client = this.clientsBattleReady[clientIndex];
    this.clientsBattleReady.splice(clientIndex, 1);
    return client.client;
  }

}
