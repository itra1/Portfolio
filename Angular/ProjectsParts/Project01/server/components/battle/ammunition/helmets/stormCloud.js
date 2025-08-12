let Helmet = require('../helmet');
let StormLight = require('../../actionsEvent/stormLight');

/**
 * Грозовое облачко
 *
 * После каждого пропущенного удара, оно наносит один случайный удар силой 1 урон, может ударить как
 * противника, так и владельца. Удар всегда проходит, минуя шлемы и щиты.
 *
 * @type {module.StormCloud}
 */
module.exports = class StormCloud extends Helmet {

  constructor() {
    super();

    this.uuid = 'e7f790e0-45a2-11e8-97e3-752988b4e4e2';

  }

  AfterDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive ||!this.HitIn(data.attack))
      return;

    let client = data.battle.clients[Math.round(Math.random())];

    client.monster.SetDamage(1);

    data.actions.push(new StormLight({
      monsterUuid: client.monster.uuid,
      damage: 1,
      live: client.monster.actualHealth
    }));
  }

};
