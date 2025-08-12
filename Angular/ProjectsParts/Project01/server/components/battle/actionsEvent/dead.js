/**
 * Смерть персонажа
 * @type {class}
 */
module.exports = class {

  constructor(inputData) {

    /** @member {number} Действие */
    this.eventId = 3;

    /** @member {string} UUID монстра */
    this.monsterUuid = inputData.monsterUuid;


    console.log(this.eventId + ' ' + JSON.stringify(inputData));

  }
};
