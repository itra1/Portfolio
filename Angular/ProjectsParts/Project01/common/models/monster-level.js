'use strict';

module.exports = function (Monsterlevel) {

  /**
   * Создание или изменение записи описания уровня
   * @param {number} level Уровень
   * @param {string} description Описание
   * @param {number} userId Id игрока
   * @param {Function(Error, object)} callback
   */

  Monsterlevel.createOrReplace = function (level, description, userId, callback) {


    Monsterlevel.findOne({where: {and: [{level: level}, {userId: userId}]}}, (err, monsterLevel) => {

      if (err)
        return callback(err, null);

      if (monsterLevel == null) {

        Monsterlevel.create(
          {
            level: level,
            userId: userId,
            description: description
          }, callback);

      } else {
        monsterLevel.description = description;
        monsterLevel.save();
        callback(null, monsterLevel);
      }

    });

  };


};
