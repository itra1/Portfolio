let Weapon = require('../weapon');
let WeaponRevertEvent = require('../../actionsEvent/weaponRevert');

/**
 * Волшебная палочка
 *
 * При попадании, наносится 1 базовый урон и игроки меняются оружием. Не ломается.
 *
 * @type {module.Brick}
 */
module.exports = class MagicWand extends Weapon{

  constructor(){
    super();

    this.uuid = '11ceaaa0-43f7-11e8-a030-8708785d77e2';

    let isChange = false;

  }

  BeforeAction(data) {

    this.isChange = false;
  }

  AfterAttack(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.HitIn(data.attack))
      return;

    super.AfterAttack(data);

    if(this.isChange) return;

    if(!attack.isBlock && attack.damage > 0 && this.specialEffect){

      // Меняем оружия местами

      let client = data.battle.clients.find(x => x.token !== data.parent.token);

      let opponentWeapon = client.ammunitions.find(x=>x.ammunition === 3 && x.IsActive);

      let myWeaponInd = data.parent.ammunitions.findIndex(x=>x.ammunition === 3);

      if(opponentWeapon != null){
        let opponentWepInd = client.ammunitions.findIndex(x=>x.ammunition ===3);
        client.ammunitions.splice(opponentWepInd,1);
      }

      client.ammunitions.push(data.parent.ammunitions[myWeaponInd]);
      data.parent.ammunitions.splice(myWeaponInd,1);
      if(opponentWeapon !== undefined)
        data.parent.ammunitions.push(opponentWeapon);

      data.actions.push(new WeaponRevertEvent());
      this.isChange = true;
    }

  }

};
