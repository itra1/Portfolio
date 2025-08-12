/**
 * Изменение состояния бумеранга
 * @type {Boomerang}
 */
module.exports = class {

  constructor(inputData){

    /** @member {number} Действие */
    this.eventId = 17;

    /** @member {string} Идентификатор монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {string} UUID монстра */
    this.isOut = inputData.isOut;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }

};
