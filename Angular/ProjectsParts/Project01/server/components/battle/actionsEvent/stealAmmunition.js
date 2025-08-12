/**
 * Кража амуниции попугаем
 * @type {class}
 */
module.exports = class {

  constructor(inputData) {
    /** @member {number} Действие */
    this.eventId = 8;

    /** @member {string} UUID монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {string} UUID монстра */
    this.ammunitionUuid = inputData.ammunitionUuid;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }
};
