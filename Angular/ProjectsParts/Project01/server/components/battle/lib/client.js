
module.exports = class Client{

  constructor(){
    /**
     * Идентификатор игрока
     * @type {string}
     */
    this.id = '';

    /**
     * пользователь
     * @type {{}}
     */
    this.user = {}


    /**
     * Токен игрока
     * @type {string}
     */
    this.token = '';
    /**
     * Активный сокет
     * @type {WebSocket}
     */
    this.ws = {};
    /**
     * Это бот
     * @type {boolean}
     */
    this.boat = false;

    /**
     * Амуниия
     * @type {Array}
     */
    this.ammunitions = [];

    /**
     * Результат последней мини игры
     * @type {null}
     */
    this.miniGame = null;

    /**
     * Время лжибания боя
     * @type {null}
     */
    this.timerBattleWait = null;
  }

  CancelBattleWait(){
    if(this.timerBattleWait)
      clearTimeout(this.timerBattleWait)
  }

};
