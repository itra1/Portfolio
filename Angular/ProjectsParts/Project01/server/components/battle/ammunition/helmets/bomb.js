let Helmet = require('../helmet');
let EnumLib = require('../../../../app/enumList');

/**
 * Бомба
 *
 * При ударе по бомбе оба монстра получают по 3 урона.
 *
 * @type {class}
 */
module.exports = class Bomb extends Helmet{

  constructor(){
    super();

    this.uuid = '1501dfc0-44d4-11e8-ab11-eb0b40b7e297';

    this.damage = 3;

    this.box = true;
  }

  BeforeDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    if(this.box)
      return;

    super.BeforeDamage(data);

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

    // Нанесение урона себе
    let client = data.battle.clients.find(x => x.token !== data.parent.token);
    data.battle.AttackEvent(data.battle.FindActionByRound(data.battle.round),
      data.parent,
      {step: data.battle.actionNumber, action: EnumLib.actionType.headHit, damage: this.damage},
      client,
      {step: data.battle.actionNumber, action: EnumLib.actionType.none}
    );

    // оппоненту
    data.battle.AttackEvent(data.battle.FindActionByRound(data.battle.round),
      client,
      {step: data.battle.actionNumber, action: EnumLib.actionType.headHit, damage: this.damage},
      data.parent,
      {step: data.battle.actionNumber, action: EnumLib.actionType.none}
    );

    super.Destroy(data, this);

  }

};
