let BombHelmet = require('./helmets/bomb');
let BowlHelmet = require('./helmets/bowl');
let CactusHelmet = require('./helmets/cactus');
let CasketHelmet = require('./helmets/casket');
let GoldenCrownHelmet = require('./helmets/goldenCrown');
let GumHelmet = require('./helmets/gum');
let IceHelmet = require('./helmets/ice');
let LeprechaunHelmet = require('./helmets/leprechaun');
let MagicBallHelmet = require('./helmets/magicBall');
let NestingDollHelmet = require('./helmets/nestingDoll');
let ParrotHelmet = require('./helmets/parrot');
let PillowHelmet = require('./helmets/pillow');
let PotionHelmet = require('./helmets/potion');
let RadBarrelHelmet = require('./helmets/radBarrel');
let StormCloudHelmet = require('./helmets/stormCloud');
let WoodenBoxHelmet = require('./helmets/woodenBox');

let BombShield = require('./shields/bomb');
let BowlShield = require('./shields/bowl');
let CactusShield = require('./shields/cactus');
let CasketShield = require('./shields/casket');
let FingerShield = require('./shields/finger');
let GoldGobletShield = require('./shields/goldGoblet');
let GumShield = require('./shields/gum');
let IceShield = require('./shields/ice');
let MagicBallShield = require('./shields/magicBall');
let MedievalShieldShield = require('./shields/medievalShield');
let NestingDollShield = require('./shields/nestingDoll');
let PacketDripShield = require('./shields/packetDrip');
let ParrotShield = require('./shields/parrot');
let PillowShield = require('./shields/pillow');
let PotionShield = require('./shields/potion');
let RadBarrelShield = require('./shields/radBarrel');
let StoneShieldShield = require('./shields/stoneShield');
let StormCloudShield = require('./shields/stormCloud');
let WoodenBoxShield = require('./shields/woodenBox');

let BoomerangWeapon = require('./weapons/boomerang');
let BrickWeapon = require('./weapons/brick');
let BroomWeapon = require('./weapons/broom');
let LadleWeapon = require('./weapons/ladle');
let MagicWandWeapon = require('./weapons/magicWand');
let NeedleWeapon = require('./weapons/needle');
let SnakeWeapon = require('./weapons/snake');
let SwordLightWeapon = require('./weapons/swordLight');
let SwordRubiesWeapon = require('./weapons/swordRubies');
let SwordVampireWeapon = require('./weapons/swordVampire');

module.exports = class AmmunitionLibrary {

  static GetAmmunition(ammunitionId) {

    switch (ammunitionId) {
      case '1501dfc0-44d4-11e8-ab11-eb0b40b7e297': return new BombHelmet();
      case '9c877b90-40f5-11e8-bcae-41e5ecbe4e9d': return new BowlHelmet();
      case 'a51eb5e0-426a-11e8-b817-b3c2e6fc9aec': return new CactusHelmet();
      case '779192e0-40fe-11e8-bcae-41e5ecbe4e9d': return new CasketHelmet();
      case '78122250-44d4-11e8-ab11-eb0b40b7e297': return new GoldenCrownHelmet();
      case '170c8e40-4610-11e8-a12c-8df78b300e5e': return new GumHelmet();
      case 'ee459450-427e-11e8-b817-b3c2e6fc9aec': return new IceHelmet();
      case 'fbd5fb30-3e84-11e8-b28a-ff0bd550df3f': return new LeprechaunHelmet();
      case 'de5e91b0-4614-11e8-a12c-8df78b300e5e': return new MagicBallHelmet();
      case '69d39630-4612-11e8-a12c-8df78b300e5e': return new NestingDollHelmet();
      case 'c404c6e0-40f6-11e8-bcae-41e5ecbe4e9d': return new ParrotHelmet();
      case '96288b40-45a5-11e8-97e3-752988b4e4e2': return new PillowHelmet();
      case '6d6ef1c0-3e85-11e8-b28a-ff0bd550df3f': return new PotionHelmet();
      case '3e20c4b0-45a7-11e8-97e3-752988b4e4e2': return new RadBarrelHelmet();
      case 'e7f790e0-45a2-11e8-97e3-752988b4e4e2': return new StormCloudHelmet();
      case '5f67edb0-40f2-11e8-bcae-41e5ecbe4e9d': return new WoodenBoxHelmet();

      case '75935b30-441a-11e8-bf03-2b2968267520': return new BombShield();
      case 'b1b7ab10-441b-11e8-bf03-2b2968267520': return new BowlShield();
      case '37301e20-44e5-11e8-ab11-eb0b40b7e297': return new CactusShield();
      case 'a0c1b5a0-44e1-11e8-ab11-eb0b40b7e297': return new CasketShield();
      case '9bbca390-433c-11e8-ada2-7b67d221757b': return new FingerShield();
      case '0d0a7cd0-433c-11e8-ada2-7b67d221757b': return new GoldGobletShield();
      case '7285cbf0-433c-11e8-ada2-7b67d221757b': return new GumShield();
      case '0ff2e5e0-44e0-11e8-ab11-eb0b40b7e297': return new IceShield();
      case 'ea9174a0-433c-11e8-ada2-7b67d221757b': return new MagicBallShield();
      case '83401180-45a3-11e8-97e3-752988b4e4e2': return new MedievalShieldShield();
      case '91b33950-4340-11e8-ada2-7b67d221757b': return new NestingDollShield();
      case '3e7f3870-4272-11e8-b817-b3c2e6fc9aec': return new PacketDripShield();
      case '5632ad30-4df8-11e8-9b88-29bace3cb1ad': return new ParrotShield();
      case '48035a30-427b-11e8-b817-b3c2e6fc9aec': return new PillowShield();
      case 'd4d06600-3e85-11e8-b28a-ff0bd550df3f': return new PotionShield();
      case '0c1771a0-4339-11e8-ada2-7b67d221757b': return new RadBarrelShield();
      case '9a2630a0-45a3-11e8-97e3-752988b4e4e2': return new StoneShieldShield();
      case '6562a2d0-4275-11e8-b817-b3c2e6fc9aec': return new StormCloudShield();
      case 'd62bd510-441c-11e8-bf03-2b2968267520': return new WoodenBoxShield();

      case 'c000b570-4404-11e8-a030-8708785d77e2': return new BoomerangWeapon();
      case '509c5420-43ee-11e8-a030-8708785d77e2': return new BrickWeapon();
      case 'b1573500-43ee-11e8-a030-8708785d77e2': return new BroomWeapon();
      case 'ded8ada0-43f9-11e8-a030-8708785d77e2': return new LadleWeapon();
      case '11ceaaa0-43f7-11e8-a030-8708785d77e2': return new MagicWandWeapon();
      case '82589bd0-43ef-11e8-a030-8708785d77e2': return new NeedleWeapon();
      case 'e13ed0d0-43fc-11e8-a030-8708785d77e2': return new SnakeWeapon();
      case 'c9d32d00-4407-11e8-a030-8708785d77e2': return new SwordLightWeapon();
      case '92f318d0-43ef-11e8-a030-8708785d77e2': return new SwordRubiesWeapon();
      case 'a4b69a90-43f1-11e8-a030-8708785d77e2': return new SwordVampireWeapon();
    }

    console.error('WS: Не определен пакет с идентификатором %s', ammunitionId);

    return null;
  }

  static GetRandomHelmet(){

    let helmetNum = Math.floor(Math.random() * 16);

    switch (helmetNum) {
      case 0: return new BombHelmet();
      case 1: return new BowlHelmet();
      case 2: return new CactusHelmet();
      case 3: return new CasketHelmet();
      case 4: return new GoldenCrownHelmet();
      case 5: return new GumHelmet();
      case 6: return new IceHelmet();
      case 7: return new LeprechaunHelmet();
      case 8: return new MagicBallHelmet();
      case 9: return new NestingDollHelmet();
      case 10: return new ParrotHelmet();
      case 11: return new PillowHelmet();
      case 12: return new PotionHelmet();
      case 13: return new RadBarrelHelmet();
      case 14: return new StormCloudHelmet();
      case 15: return new WoodenBoxHelmet();
    }

  }

  static GetRandomShield(){

    let shieldNum = Math.floor(Math.random() * 19);

    switch (shieldNum) {
      case 0: return new BombShield();
      case 1: return new BowlShield();
      case 2: return new CactusShield();
      case 3: return new CasketShield();
      case 4: return new FingerShield();
      case 5: return new GoldGobletShield();
      case 6: return new GumShield();
      case 7: return new IceShield();
      case 8: return new MagicBallShield();
      case 9: return new MedievalShieldShield();
      case 10: return new NestingDollShield();
      case 11: return new PacketDripShield();
      case 12: return new ParrotShield();
      case 13: return new PillowShield();
      case 14: return new PotionShield();
      case 15: return new RadBarrelShield();
      case 16: return new StoneShieldShield();
      case 17: return new StormCloudShield();
      case 18: return new WoodenBoxShield();
    }
  }

  static GetRandomWeapon(){

    let weaponNum = Math.floor(Math.random() * 10);

    switch (weaponNum) {
      case 0: return new BoomerangWeapon();
      case 1: return new BrickWeapon();
      case 2: return new BroomWeapon();
      case 3: return new LadleWeapon();
      case 4: return new MagicWandWeapon();
      case 5: return new NeedleWeapon();
      case 6: return new SnakeWeapon();
      case 7: return new SwordLightWeapon();
      case 8: return new SwordRubiesWeapon();
      case 9: return new SwordVampireWeapon();
    }
  }

};
