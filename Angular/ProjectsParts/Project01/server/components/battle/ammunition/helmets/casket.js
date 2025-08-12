let Helmet = require('../helmet');
let EnumLib = require('../../../../app/enumList');

/**
 * Шкатулка с высунутой боксерской перчаткой на пружине.
 *
 * При попадании по ней, блокирует 2 урона и наносит ответный удар на 1 урон. (Если шкатулка на голове - то в голову, если в руке - то в тело) Выдерживает 2 удара по себе и пропадает.
 *
 * @type {module.Bowl}
 */
module.exports = class Casket extends Helmet {

  constructor() {
    super();

    this.uuid = '779192e0-40fe-11e8-bcae-41e5ecbe4e9d';

    /**
     * Количество ударов, которые выдерживат
     * @type {number}
     */
    this.health = 2;

    /**
     * Количество блокируемого урона
     * @type {number}
     */
    this.blockHit = 2;

    /**
     * Ответный урон
     * @type {number}
     */
    this.reversHit = 1;

    this.box = true;
  }

  BeforeDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);

    if(this.box)
      return;

    this.health--;

    // Блокирует урон
    attack.damage -= this.blockHit;

  }

  AfterDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.AfterDamage(data, attack);

    // Уничтожаем бокс
    if(this.box){
      this.BoxDestroy(data,this);
      this.box = false;
      return;
    }

    // Нанесение урона
    let client = data.battle.clients.find(x => x.token !== data.parent.token);
    data.battle.AttackEvent(data.battle.FindActionByRound(data.battle.round),
      data.parent,
      {step: data.battle.actionNumber, action: EnumLib.actionType.headHit},
      client,
      {step: data.battle.actionNumber, action: EnumLib.actionType.none}
    );

    if (this.health <= 0) {
      super.Destroy(data, this);
    }

  }

};
