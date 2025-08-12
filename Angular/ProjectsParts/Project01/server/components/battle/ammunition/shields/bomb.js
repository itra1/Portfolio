let Shield = require('../shield');
let EnumLib = require('../../../../app/enumList');

/**
 * Бомба
 *
 * При ударе по бомбе оба монстра получают по 3 урона.
 *
 * @type {module.Bomb}
 */
module.exports = class Bomb extends Shield{

  constructor(){
    super();

    this.uuid = '75935b30-441a-11e8-bf03-2b2968267520';

    this.damage = 3;

    this.box = true;
  }

  BeforeDamage(data,attack) {

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
