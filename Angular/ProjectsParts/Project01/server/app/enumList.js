/**
 * Типы атаки
 * @type {{none: number, headBlock: number, bodyBlock: number, headHit: number, bodyHit: number}}
 */
module.exports.actionType = {
  none: 0,                // Не использовано действие
  headBlock: 1,           // Блок головы
  bodyBlock: 2,           // Блок живота
  headHit: 3,             // Удар в голову
  bodyHit: 4              // Удар в живот
};
