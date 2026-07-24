using BaseLib.Abstracts;
using BaseLib.Utils;
using Tifa.TifaCode.Character;

namespace Tifa.TifaCode.Potions;

[Pool(typeof(TifaPotionPool))]
public abstract class TifaPotion : CustomPotionModel;