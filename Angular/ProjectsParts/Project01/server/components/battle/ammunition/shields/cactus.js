let Shield = require('../shield');
let EnumLib = require('../../../../app/enumList');
let CactusDestroyEvent = require('../../actionsEvent/cactusDestroy');

/**
 * Кактус с тремя стволами.
 *
 * Каждое попадание по нему наносит 1 урон противнику. И отваливается часть кактуса. Пропадает после трех попаданий по нему. Не блокирует урон.
 *
 * @type {module.Bowl}
 */
module.exports = class Cactus extends Shield{

  constructor(){
    super();

    this.uuid = '37301e20-44e5-11e8-ab11-eb0b40b7e297';

    this.health = 3;
  }

  BeforeDamage(data, sttack){

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);

    this.health--;

    data.actions.push(new CactusDestroyEvent({
      monsterUuid: data.parent.monster.uuid,
      uuid: this.uuid,
      health: this.health
    }));

  }

  AfterDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.HitIn(data.attack))
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

    // Уничтожаем, если защита окончена
    if(this.health <= 0){
      super.Destroy(data,this);
    }

  }

};
