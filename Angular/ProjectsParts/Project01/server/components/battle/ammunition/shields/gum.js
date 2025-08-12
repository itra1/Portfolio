let Shield = require('../shield');
let GumActivate = require('../../actionsEvent/gumActive');

/**
 * Жвачка
 *
 * Блокирует один урон и если у игрока есть оружие, то оно облепляется жвачкой и не действует 3 хода (бонусы от
 * оружия игнорируется, но базовый урон проходит), после чего жвачка пропадает.
 *
 * @type {Gum}
 */
module.exports = class Gum extends Shield {

  constructor() {
    super();

    this.uuid = '7285cbf0-433c-11e8-ada2-7b67d221757b';

    this.health = 4;

    this.opponentLocked = false;
    this.ammunit = null;
  }

  BeforeDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data, attack);

    let client = data.battle.clients.find(x => x.token !== data.parent.token);

    if (!this.opponentLocked) {

      attack.damage--;

      if (this.ammunit == null && client.ammunitions.length > 0)
        this.ammunit = client.ammunitions.find(x => x.StillReady && x.ammunition === 3);

      if(this.ammunit != null){
        this.opponentLocked = true;
        this.ammunit.specialEffect = false;
        data.actions.push(new GumActivate({
          monsterUuid: client.monster.uuid,
          weaponUuid: this.ammunit.uuid,
          isActivate: true
        }));
      }else{
        this.health = 0;
      }

    }

  }

  AfterAction(data){

    if(this.opponentLocked){
      this.health--;
    }

    if(!this.IsActive)
      return;

    super.AfterAction(data);
    // Уничтожаем, если защита окончена
    if(this.health <= 0){
      this.ammunit.specialEffect = true;

      let client = data.battle.clients.find(x => x.token !== data.parent.token);
      data.events.push(new GumActivate({
        monsterUuid: client.monster.uuid,
        weaponUuid: this.ammunit.uuid,
        isActivate: false
      }));

      super.Destroy({
        battle: data.battle,         // Бой
        parent: data.parent, // Владелец предмета
        actions: data.actions.result.find(x => x.number === this.actionNumber).actions,         // Действия
        attack: data.parent // Атака
      },this);
    }

  }

};
