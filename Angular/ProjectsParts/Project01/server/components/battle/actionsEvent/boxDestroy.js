/**
 * Уничтожение ящика
 * @type {class}
 */
module.exports = class {

  constructor(inputData) {

    /** @member {number} Действие */
    this.eventId = 6;

    /** @member {string} UUID монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {string} Идентификатор амуниции которая была в ящике */
    this.ammunitionUuid = inputData.uuid;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }
};
