/**
 * Инвертируются действия героя
 * @type {class}
 */
module.exports = class {

  constructor(inputData) {

    /** @member {number} Действие */
    this.eventId = 5;

    /** @member {string} UUID монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {string} UUID уничтоженной амуниции */
    this.ammunitionUuid = inputData.uuid;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }
};
