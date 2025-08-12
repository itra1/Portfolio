/**
 * Использование жвачки
 * @type {class}
 */
module.exports = class {

  constructor(inputData) {

    /** @member {number} Действие */
    this.eventId = 14;

    /** @member {string} UUID монстра у которого заморожено оружие */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {number} UUID замороженного оружия */
    this.weaponUuid = inputData.weaponUuid;

    /** @member {number} Жвачка прилипла  */
    this.isActivate = inputData.isActivate;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }

};
