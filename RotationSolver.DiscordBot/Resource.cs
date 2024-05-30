namespace RotationSolver.DiscordBot;

internal static class Resource
{
    public const string DefaultDuty = "Another Aloalo Island (Savage)";

    public static readonly List<(string, uint, byte, byte, byte)> DutyAndImage =
    [
        ("the Thousand Maws of TotoRak", 112005, 1, 1, 2),
        ("the TamTara Deepcroft", 112002, 1, 1, 2),
        ("Copperbell Mines", 112003, 1, 1, 2),
        ("Sastasha", 112001, 1, 1, 2),
        ("the Aurum Vale", 112014, 1, 1, 2),
        ("Haukke Manor", 112006, 1, 1, 2),
        ("Halatali", 112004, 1, 1, 2),
        ("Brayflox's Longstop", 112007, 1, 1, 2),
        ("the Sunken Temple of Qarn", 112009, 1, 1, 2),
        ("the Wanderer's Palace", 112010, 1, 1, 2),
        ("the Stone Vigil", 112012, 1, 1, 2),
        ("Cutter's Cry", 112011, 1, 1, 2),
        ("Dzemael Darkhold", 112013, 1, 1, 2),
        ("Amdapor Keep", 112015, 1, 1, 2),
        ("Castrum Meridianum", 112016, 1, 1, 2),
        ("the Praetorium", 112017, 1, 1, 2),
        ("Pharos Sirius", 112027, 1, 1, 2),
        ("Copperbell Mines (Hard)", 112025, 1, 1, 2),
        ("Haukke Manor (Hard)", 112026, 1, 1, 2),
        ("Brayflox's Longstop (Hard)", 112048, 1, 1, 2),
        ("Halatali (Hard)", 112049, 1, 1, 2),
        ("the Lost City of Amdapor", 112042, 1, 1, 2),
        ("Hullbreaker Isle", 112058, 1, 1, 2),
        ("the TamTara Deepcroft (Hard)", 112059, 1, 1, 2),
        ("the Stone Vigil (Hard)", 112060, 1, 1, 2),
        ("the Sunken Temple of Qarn (Hard)", 112068, 1, 1, 2),
        ("Snowcloak", 112066, 1, 1, 2),
        ("Sastasha (Hard)", 112067, 1, 1, 2),
        ("Amdapor Keep (Hard)", 112077, 1, 1, 2),
        ("the Wanderer's Palace (Hard)", 112078, 1, 1, 2),
        ("the Great Gubal Library", 112091, 1, 1, 2),
        ("the Keeper of the Lake", 112076, 1, 1, 2),
        ("Neverreap", 112093, 1, 1, 2),
        ("the Vault", 112090, 1, 1, 2),
        ("the Fractal Continuum", 112094, 1, 1, 2),
        ("the Dusk Vigil", 112087, 1, 1, 2),
        ("Sohm Al", 112088, 1, 1, 2),
        ("the Aetherochemical Research Facility", 112092, 1, 1, 2),
        ("the Aery", 112089, 1, 1, 2),
        ("Pharos Sirius (Hard)", 112117, 1, 1, 2),
        ("Saint Mocianne's Arboretum", 112116, 1, 1, 2),
        ("Basic Training: Enemy Parties", 111006, 1, 1, 2),
        ("Under the Armor", 111001, 1, 1, 2),
        ("Basic Training: Enemy Strongholds", 111007, 1, 1, 2),
        ("Hero on the Half Shell", 111008, 1, 1, 2),
        ("Pulling Poison Posies", 111002, 1, 1, 2),
        ("Stinging Back", 111003, 1, 1, 2),
        ("All's Well that Ends in the Well", 111005, 1, 1, 2),
        ("Flicking Sticks and Taking Names", 111004, 1, 1, 2),
        ("More than a Feeler", 111009, 1, 1, 2),
        ("Annoy the Void", 111010, 1, 1, 2),
        ("Shadow and Claw", 111011, 1, 1, 2),
        ("Long Live the Queen", 111012, 1, 1, 2),
        ("Ward Up", 111013, 1, 1, 2),
        ("Solemn Trinity", 111014, 2, 2, 4),
        ("the Bowl of Embers", 112008, 1, 1, 2),
        ("the Navel", 112018, 1, 1, 2),
        ("the Howling Eye", 112019, 1, 1, 2),
        ("the Bowl of Embers (Hard)", 112021, 2, 2, 4),
        ("the Navel (Hard)", 112022, 2, 2, 4),
        ("the Howling Eye (Hard)", 112023, 2, 2, 4),
        ("the Bowl of Embers (Extreme)", 112028, 2, 2, 4),
        ("the Navel (Extreme)", 112029, 2, 2, 4),
        ("the Howling Eye (Extreme)", 112030, 2, 2, 4),
        ("Thornmarch (Hard)", 112031, 2, 2, 4),
        ("Thornmarch (Extreme)", 112050, 2, 2, 4),
        ("the Minstrel's Ballad: Ultima's Bane", 112032, 2, 2, 4),
        ("The Gilded Araya", 112536, 2, 2, 4),
        ("Special Event I", 112320, 2, 2, 4),
        ("Special Event II", 112358, 2, 2, 4),
        ("the Whorleater (Hard)", 112051, 2, 2, 4),
        ("the Whorleater (Extreme)", 112052, 2, 2, 4),
        ("A Relic Reborn: the Chimera", 112053, 2, 2, 4),
        ("A Relic Reborn: the Hydra", 112055, 2, 2, 4),
        ("Battle on the Big Bridge", 112054, 2, 2, 4),
        ("the Striking Tree (Hard)", 112062, 2, 2, 4),
        ("the Striking Tree (Extreme)", 112063, 2, 2, 4),
        ("the Akh Afah Amphitheatre (Hard)", 112073, 2, 2, 4),
        ("the Akh Afah Amphitheatre (Extreme)", 112074, 2, 2, 4),
        ("the Dragon's Neck", 112075, 2, 2, 4),
        ("Urth's Fount", 112065, 2, 2, 4),
        ("the Chrysalis", 112081, 2, 2, 4),
        ("Battle in the Big Keep", 112080, 2, 2, 4),
        ("Thok ast Thok (Hard)", 112103, 2, 2, 4),
        ("Thok ast Thok (Extreme)", 112107, 2, 2, 4),
        ("the Limitless Blue (Hard)", 112104, 2, 2, 4),
        ("the Limitless Blue (Extreme)", 112105, 2, 2, 4),
        ("the Singularity Reactor", 112106, 2, 2, 4),
        ("the Minstrel's Ballad: Thordan's Reign", 112122, 2, 2, 4),
        ("the Labyrinth of the Ancients", 112033, 1, 2, 5),
        ("the Binding Coil of Bahamut - Turn 1", 112043, 2, 2, 4),
        ("the Binding Coil of Bahamut - Turn 2", 112044, 2, 2, 4),
        ("the Binding Coil of Bahamut - Turn 3", 112045, 2, 2, 4),
        ("the Binding Coil of Bahamut - Turn 4", 112046, 2, 2, 4),
        ("the Binding Coil of Bahamut - Turn 5", 112047, 2, 2, 4),
        ("the Second Coil of Bahamut - Turn 1", 112038, 2, 2, 4),
        ("the Second Coil of Bahamut - Turn 2", 112039, 2, 2, 4),
        ("the Second Coil of Bahamut - Turn 3", 112040, 2, 2, 4),
        ("the Second Coil of Bahamut - Turn 4", 112041, 2, 2, 4),
        ("Syrcus Tower", 112061, 1, 2, 5),
        ("the Second Coil of Bahamut (Savage) - Turn 1", 112038, 2, 2, 4),
        ("the Second Coil of Bahamut (Savage) - Turn 2", 112039, 2, 2, 4),
        ("the Second Coil of Bahamut (Savage) - Turn 3", 112040, 2, 2, 4),
        ("the Second Coil of Bahamut (Savage) - Turn 4", 112041, 2, 2, 4),
        ("the Final Coil of Bahamut - Turn 1", 112069, 2, 2, 4),
        ("the Final Coil of Bahamut - Turn 2", 112070, 2, 2, 4),
        ("the Final Coil of Bahamut - Turn 3", 112071, 2, 2, 4),
        ("the Final Coil of Bahamut - Turn 4", 112072, 2, 2, 4),
        ("the World of Darkness", 112079, 1, 2, 5),
        ("Alexander - The Fist of the Father", 112095, 2, 2, 4),
        ("Alexander - The Cuff of the Father", 112096, 2, 2, 4),
        ("Alexander - The Arm of the Father", 112097, 2, 2, 4),
        ("Alexander - The Burden of the Father", 112098, 2, 2, 4),
        ("Alexander - The Fist of the Father (Savage)", 112099, 2, 2, 4),
        ("Alexander - The Cuff of the Father (Savage)", 112100, 2, 2, 4),
        ("Alexander - The Arm of the Father (Savage)", 112101, 2, 2, 4),
        ("Alexander - The Burden of the Father (Savage)", 112102, 2, 2, 4),
        ("the Void Ark", 112118, 1, 2, 5),
        ("the Borderland Ruins (Secure)", 112064, 0, 0, 0),
        ("Seal Rock (Seize)", 112108, 0, 0, 0),
        ("the Diadem (Easy)", 112119, 0, 0, 0),
        ("the Diadem", 112384, 0, 0, 0),
        ("the Diadem (Hard)", 112121, 0, 0, 0),
        ("Containment Bay S1T7", 112126, 2, 2, 4),
        ("Containment Bay S1T7 (Extreme)", 112127, 2, 2, 4),
        ("Alexander - The Fist of the Son", 112128, 2, 2, 4),
        ("Alexander - The Cuff of the Son", 112129, 2, 2, 4),
        ("Alexander - The Arm of the Son", 112130, 2, 2, 4),
        ("Alexander - The Burden of the Son", 112131, 2, 2, 4),
        ("the Lost City of Amdapor (Hard)", 112124, 1, 1, 2),
        ("the Antitower", 112125, 1, 1, 2),
        ("Alexander - The Fist of the Son (Savage)", 112132, 2, 2, 4),
        ("Alexander - The Cuff of the Son (Savage)", 112133, 2, 2, 4),
        ("Alexander - The Arm of the Son (Savage)", 112134, 2, 2, 4),
        ("Alexander - The Burden of the Son (Savage)", 112135, 2, 2, 4),
        ("Avoid Area of Effect Attacks", 112141, 0, 0, 0),
        ("Execute a Combo to Increase Enmity", 112141, 0, 0, 0),
        ("Execute a Combo in Battle", 112141, 0, 0, 0),
        ("Accrue Enmity from Multiple Targets", 112141, 0, 0, 0),
        ("Engage Multiple Targets", 112141, 0, 0, 0),
        ("Execute a Ranged Attack to Increase Enmity", 112141, 0, 0, 0),
        ("Engage Enemy Reinforcements", 112141, 0, 0, 0),
        ("Assist Allies in Defeating a Target", 112141, 0, 0, 0),
        ("Defeat an Occupied Target", 112141, 0, 0, 0),
        ("Avoid Engaged Targets", 112141, 0, 0, 0),
        ("Interact with the Battlefield", 112141, 0, 0, 0),
        ("Heal an Ally", 112141, 0, 0, 0),
        ("Heal Multiple Allies", 112141, 0, 0, 0),
        ("Final Exercise", 112141, 0, 0, 0),
        ("a Spectacle for the Ages", 112145, 0, 0, 0),
        ("the Weeping City of Mhach", 112162, 1, 2, 5),
        ("the Final Steps of Faith", 112160, 2, 2, 4),
        ("the Minstrel's Ballad: Nidhogg's Rage", 112161, 2, 2, 4),
        ("Sohr Khai", 112163, 1, 1, 2),
        ("Hullbreaker Isle (Hard)", 112164, 1, 1, 2),
        ("the Palace of the Dead (Floors 1-10)", 112166, 0, 0, 0),
        ("the Palace of the Dead (Floors 11-20)", 112167, 0, 0, 0),
        ("the Palace of the Dead (Floors 21-30)", 112168, 0, 0, 0),
        ("the Palace of the Dead (Floors 31-40)", 112169, 0, 0, 0),
        ("the Palace of the Dead (Floors 41-50)", 112170, 0, 0, 0),
        ("the Fields of Glory (Shatter)", 112165, 0, 0, 0),
        ("the Haunted Manor", 112196, 0, 0, 0),
        ("Xelphatol", 112186, 1, 1, 2),
        ("Containment Bay P1T6", 112201, 2, 2, 4),
        ("Containment Bay P1T6 (Extreme)", 112202, 2, 2, 4),
        ("Alexander - The Eyes of the Creator", 112188, 2, 2, 4),
        ("Alexander - The Breath of the Creator", 112189, 2, 2, 4),
        ("Alexander - The Heart of the Creator", 112190, 2, 2, 4),
        ("Alexander - The Soul of the Creator", 112191, 2, 2, 4),
        ("Alexander - The Eyes of the Creator (Savage)", 112192, 2, 2, 4),
        ("Alexander - The Breath of the Creator (Savage)", 112193, 2, 2, 4),
        ("Alexander - The Heart of the Creator (Savage)", 112194, 2, 2, 4),
        ("Alexander - The Soul of the Creator (Savage)", 112195, 2, 2, 4),
        ("the Triple Triad Battlehall", 112197, 0, 0, 0),
        ("the Great Gubal Library (Hard)", 112187, 1, 1, 2),
        ("LoVM: Player Battle (RP)", 112199, 0, 0, 0),
        ("LoVM: Tournament", 112200, 0, 0, 0),
        ("LoVM: Player Battle (Non-RP)", 112198, 0, 0, 0),
        ("the Diadem Hunting Grounds (Easy)", 112119, 1, 2, 5),
        ("the Diadem Hunting Grounds", 112120, 1, 2, 5),
        ("the Palace of the Dead (Floors 51-60)", 112171, 0, 0, 0),
        ("the Palace of the Dead (Floors 61-70)", 112172, 0, 0, 0),
        ("the Palace of the Dead (Floors 71-80)", 112173, 0, 0, 0),
        ("the Palace of the Dead (Floors 81-90)", 112174, 0, 0, 0),
        ("the Palace of the Dead (Floors 91-100)", 112175, 0, 0, 0),
        ("the Palace of the Dead (Floors 101-110)", 112176, 0, 0, 0),
        ("the Palace of the Dead (Floors 111-120)", 112177, 0, 0, 0),
        ("the Palace of the Dead (Floors 121-130)", 112178, 0, 0, 0),
        ("the Palace of the Dead (Floors 131-140)", 112179, 0, 0, 0),
        ("the Palace of the Dead (Floors 141-150)", 112180, 0, 0, 0),
        ("the Palace of the Dead (Floors 151-160)", 112181, 0, 0, 0),
        ("the Palace of the Dead (Floors 161-170)", 112182, 0, 0, 0),
        ("the Palace of the Dead (Floors 171-180)", 112183, 0, 0, 0),
        ("the Palace of the Dead (Floors 181-190)", 112184, 0, 0, 0),
        ("the Palace of the Dead (Floors 191-200)", 112185, 0, 0, 0),
        ("Baelsar's Wall", 112214, 1, 1, 2),
        ("Dun Scaith", 112203, 1, 2, 5),
        ("Sohm Al (Hard)", 112215, 1, 1, 2),
        ("Containment Bay Z1T9", 112212, 2, 2, 4),
        ("Containment Bay Z1T9 (Extreme)", 112213, 2, 2, 4),
        ("the Diadem - Trials of the Fury", 112221, 1, 2, 5),
        ("the Diadem - Trials of the Matron", 112222, 0, 0, 0),
        ("Shisui of the Violet Tides", 112227, 1, 1, 2),
        ("The Temple of the Fist", 112233, 1, 1, 2),
        ("the Sirensong Sea", 112226, 1, 1, 2),
        ("the Royal Menagerie", 112244, 2, 2, 4),
        ("Bardam's Mettle", 112228, 1, 1, 2),
        ("Doma Castle", 112229, 1, 1, 2),
        ("Castrum Abania", 112230, 1, 1, 2),
        ("the Pool of Tribute", 112242, 2, 2, 4),
        ("the Pool of Tribute (Extreme)", 112245, 2, 2, 4),
        ("Ala Mhigo", 112231, 1, 1, 2),
        ("Deltascape V1.0", 112234, 2, 2, 4),
        ("Deltascape V2.0", 112235, 2, 2, 4),
        ("Deltascape V3.0", 112236, 2, 2, 4),
        ("Deltascape V4.0", 112237, 2, 2, 4),
        ("Deltascape V1.0 (Savage)", 112238, 2, 2, 4),
        ("Deltascape V2.0 (Savage)", 112239, 2, 2, 4),
        ("Deltascape V3.0 (Savage)", 112240, 2, 2, 4),
        ("Deltascape V4.0 (Savage)", 112241, 2, 2, 4),
        ("Kugane Castle", 112232, 1, 1, 2),
        ("Emanation", 112243, 2, 2, 4),
        ("Emanation (Extreme)", 112246, 2, 2, 4),
        ("Astragalos", 112257, 0, 0, 0),
        ("the Minstrel's Ballad: Shinryu's Domain", 112258, 2, 2, 4),
        ("the Drowned City of Skalla", 112255, 1, 1, 2),
        ("the Unending Coil of Bahamut (Ultimate)", 112261, 2, 2, 4),
        ("the Royal City of Rabanastre", 112256, 1, 2, 5),
        ("the Forbidden Land, Eureka Anemos", 112275, 0, 0, 0),
        ("Hells' Lid", 112264, 1, 1, 2),
        ("the Fractal Continuum (Hard)", 112263, 1, 1, 2),
        ("Sigmascape V1.0", 112265, 2, 2, 4),
        ("Sigmascape V2.0", 112267, 2, 2, 4),
        ("Sigmascape V3.0", 112269, 2, 2, 4),
        ("Sigmascape V4.0", 112271, 2, 2, 4),
        ("the Jade Stoa", 112273, 2, 2, 4),
        ("the Jade Stoa (Extreme)", 112274, 2, 2, 4),
        ("Sigmascape V1.0 (Savage)", 112266, 2, 2, 4),
        ("Sigmascape V2.0 (Savage)", 112268, 2, 2, 4),
        ("Sigmascape V3.0 (Savage)", 112270, 2, 2, 4),
        ("Sigmascape V4.0 (Savage)", 112272, 2, 2, 4),
        ("the Valentione's Ceremony", 112262, 0, 0, 0),
        ("the Great Hunt", 112289, 2, 2, 4),
        ("the Great Hunt (Extreme)", 112290, 1, 1, 2),
        ("Chocobo Race: Tutorial", 112086, 0, 0, 0),
        ("Race 1 - Hugging the Inside", 112085, 0, 0, 0),
        ("Race 2 - Keep Away", 112084, 0, 0, 0),
        ("Race 3 - Inability", 112086, 0, 0, 0),
        ("Race 4 - Heavy Hooves", 112085, 0, 0, 0),
        ("Race 5 - Defending the Rush", 112084, 0, 0, 0),
        ("Race 6 - Road Rivals", 112086, 0, 0, 0),
        ("Race 7 - Field of Dreams", 112084, 0, 0, 0),
        ("Race 8 - Playing Both Ends", 112085, 0, 0, 0),
        ("Race 9 - Stamina", 112086, 0, 0, 0),
        ("Race 10 - Cat and Mouse", 112084, 0, 0, 0),
        ("Race 11 - Mad Dash", 112086, 0, 0, 0),
        ("Race 12 - Bag of Tricks", 112085, 0, 0, 0),
        ("Race 13 - Tag Team", 112086, 0, 0, 0),
        ("Race 14 - Heavier Hooves", 112084, 0, 0, 0),
        ("Race 15 - Ultimatum", 112085, 0, 0, 0),
        ("Chocobo Race: Sagolii Road", 112086, 0, 0, 0),
        ("Chocobo Race: Costa del Sol", 112084, 0, 0, 0),
        ("Chocobo Race: Tranquil Paths", 112085, 0, 0, 0),
        ("the Swallow's Compass", 112283, 1, 1, 2),
        ("Castrum Fluminis", 112291, 2, 2, 4),
        ("the Minstrel's Ballad: Tsukuyomi's Pain", 112292, 2, 2, 4),
        ("the Weapon's Refrain (Ultimate)", 112296, 2, 2, 4),
        ("Heaven-on-High  (Floors 1-10)", 112298, 0, 0, 0),
        ("Heaven-on-High  (Floors 11-20)", 112299, 0, 0, 0),
        ("Heaven-on-High  (Floors 21-30)", 112300, 0, 0, 0),
        ("Heaven-on-High  (Floors 31-40)", 112301, 0, 0, 0),
        ("Heaven-on-High  (Floors 41-50)", 112302, 0, 0, 0),
        ("Heaven-on-High  (Floors 51-60)", 112303, 0, 0, 0),
        ("Heaven-on-High  (Floors 61-70)", 112304, 0, 0, 0),
        ("Heaven-on-High  (Floors 71-80)", 112305, 0, 0, 0),
        ("Heaven-on-High  (Floors 81-90)", 112306, 0, 0, 0),
        ("Heaven-on-High  (Floors 91-100)", 112307, 0, 0, 0),
        ("the Ridorana Lighthouse", 112286, 1, 2, 5),
        ("Stage 1: Tutorial", 112114, 0, 0, 0),
        ("Stage 2: Hatching a Plan", 112114, 0, 0, 0),
        ("Stage 3: The First Move", 112115, 0, 0, 0),
        ("Stage 4: Little Big Beast", 112115, 0, 0, 0),
        ("Stage 5: Turning Tribes", 112115, 0, 0, 0),
        ("Stage 6: Off the Deepcroft", 112115, 0, 0, 0),
        ("Stage 7: Rivals", 112115, 0, 0, 0),
        ("Stage 8: Always Darkest", 112115, 0, 0, 0),
        ("Stage 9: Mine Your Minions", 112115, 0, 0, 0),
        ("Stage 10: Children of Mandragora", 112115, 0, 0, 0),
        ("Stage 11: The Queen and I", 112115, 0, 0, 0),
        ("Stage 12: Breakout", 112115, 0, 0, 0),
        ("Stage 13: My Name Is Cid", 112115, 0, 0, 0),
        ("Stage 14: Like a Nut", 112115, 0, 0, 0),
        ("Stage 15: Urth's Spout", 112115, 0, 0, 0),
        ("Stage 16: Exodus", 112115, 0, 0, 0),
        ("Stage 17: Over the Wall", 112115, 0, 0, 0),
        ("Stage 18: The Hunt", 112115, 0, 0, 0),
        ("Stage 19: Battle on the Bitty Bridge", 112115, 0, 0, 0),
        ("Stage 20: Guiding Light", 112115, 0, 0, 0),
        ("Stage 21: Wise Words", 112115, 0, 0, 0),
        ("Stage 22: World of Poor Lighting", 112115, 0, 0, 0),
        ("Stage 23: The Binding Coil", 112115, 0, 0, 0),
        ("Stage 24: The Final Coil", 112115, 0, 0, 0),
        ("LoVM: Master Battle", 112109, 0, 0, 0),
        ("LoVM: Master Battle (Hard)", 112110, 0, 0, 0),
        ("LoVM: Master Battle (Extreme)", 112111, 0, 0, 0),
        ("LoVM: Master Tournament", 112200, 0, 0, 0),
        ("the Forbidden Land, Eureka Pagos", 112308, 0, 0, 0),
        ("the Calamity Retold", 112297, 0, 0, 0),
        ("Saint Mocianne's Arboretum (Hard)", 112310, 1, 1, 2),
        ("the Burn", 112311, 1, 1, 2),
        ("Alphascape V1.0", 112312, 2, 2, 4),
        ("Alphascape V2.0", 112313, 2, 2, 4),
        ("Alphascape V3.0", 112314, 2, 2, 4),
        ("Alphascape V4.0", 112315, 2, 2, 4),
        ("Alphascape V1.0 (Savage)", 112316, 2, 2, 4),
        ("Alphascape V2.0 (Savage)", 112317, 2, 2, 4),
        ("Alphascape V3.0 (Savage)", 112318, 2, 2, 4),
        ("Alphascape V4.0 (Savage)", 112319, 2, 2, 4),
        ("Kugane Ohashi", 112320, 2, 2, 4),
        ("Hells' Kier", 112321, 2, 2, 4),
        ("Hells' Kier (Extreme)", 112322, 2, 2, 4),
        ("the Forbidden Land, Eureka Pyros", 112323, 0, 0, 0),
        ("Hidden Gorge", 112335, 0, 0, 0),
        ("Leap of Faith", 112520, 0, 0, 0),
        ("All's Well That Starts Well", 112332, 0, 0, 0),
        ("the Ghimlyt Dark", 112333, 1, 1, 2),
        ("Much Ado About Pudding", 112332, 0, 0, 0),
        ("Waiting for Golem", 112332, 0, 0, 0),
        ("Gentlemen Prefer Swords", 112332, 0, 0, 0),
        ("The Threepenny Turtles", 112332, 0, 0, 0),
        ("Eye Society", 112332, 0, 0, 0),
        ("A Chorus Slime", 112332, 0, 0, 0),
        ("Bomb-edy of Errors", 112332, 0, 0, 0),
        ("To Kill a Mockingslime", 112332, 0, 0, 0),
        ("A Little Knight Music", 112332, 0, 0, 0),
        ("Some Like It Excruciatingly Hot", 112332, 0, 0, 0),
        ("The Plant-om of the Opera", 112332, 0, 0, 0),
        ("Beauty and a Beast", 112332, 0, 0, 0),
        ("Blobs in the Woods", 112332, 0, 0, 0),
        ("The Me Nobody Nodes", 112332, 0, 0, 0),
        ("Sunset Bull-evard", 112332, 0, 0, 0),
        ("The Sword of Music", 112332, 0, 0, 0),
        ("Midsummer Night's Explosion", 112332, 0, 0, 0),
        ("On a Clear Day You Can Smell Forever", 112332, 0, 0, 0),
        ("Miss Typhon", 112332, 0, 0, 0),
        ("Chimera on a Hot Tin Roof", 112332, 0, 0, 0),
        ("Here Comes the Boom", 112332, 0, 0, 0),
        ("Behemoths and Broomsticks", 112332, 0, 0, 0),
        ("Amazing Technicolor Pit Fiends", 112332, 0, 0, 0),
        ("Dirty Rotten Azulmagia", 112332, 0, 0, 0),
        ("the Orbonne Monastery", 112334, 1, 2, 5),
        ("the Wreath of Snakes", 112339, 2, 2, 4),
        ("the Wreath of Snakes (Extreme)", 112340, 2, 2, 4),
        ("the Forbidden Land, Eureka Hydatos", 112338, 0, 0, 0),
        ("Air Force One", 112337, 0, 0, 0),
        ("Novice Mahjong (Full Ranked Match)", 112336, 0, 0, 0),
        ("Advanced Mahjong (Full Ranked Match)", 112336, 0, 0, 0),
        ("Four-player Mahjong (Full Match, Kuitan Enabled)", 112336, 0, 0, 0),
        ("Dohn Mheg", 112343, 1, 1, 2),
        ("Four-player Mahjong (Full Match, Kuitan Disabled)", 112336, 0, 0, 0),
        ("the Qitana Ravel", 112344, 1, 1, 2),
        ("Amaurot", 112347, 1, 1, 2),
        ("Eden's Gate: Resurrection", 112350, 2, 2, 4),
        ("Eden's Gate: Resurrection (Savage)", 112351, 2, 2, 4),
        ("the Twinning", 112348, 1, 1, 2),
        ("Malikah's Well", 112345, 1, 1, 2),
        ("The Dancing Plague", 112358, 2, 2, 4),
        ("the Dancing Plague (Extreme)", 112359, 2, 2, 4),
        ("Mt. Gulg", 112346, 1, 1, 2),
        ("Akadaemia Anyder", 112349, 1, 1, 2),
        ("The Crown of the Immaculate", 112360, 2, 2, 4),
        ("the Crown of the Immaculate (Extreme)", 112361, 2, 2, 4),
        ("Holminster Switch", 112342, 1, 1, 2),
        ("Eden's Gate: Inundation", 112354, 2, 2, 4),
        ("Eden's Gate: Inundation (Savage)", 112355, 2, 2, 4),
        ("Eden's Gate: Descent", 112352, 2, 2, 4),
        ("Eden's Gate: Descent (Savage)", 112353, 2, 2, 4),
        ("The Dying Gasp", 112362, 2, 2, 4),
        ("Eden's Gate: Sepulture", 112356, 2, 2, 4),
        ("Eden's Gate: Sepulture (Savage)", 112357, 2, 2, 4),
        ("The Grand Cosmos", 112373, 1, 1, 2),
        ("The Minstrel's Ballad: Hades's Elegy", 112372, 2, 2, 4),
        ("The Epic of Alexander (Ultimate)", 112374, 2, 2, 4),
        ("Papa Mia", 112332, 0, 0, 0),
        ("Lock up Your Snorters", 112332, 0, 0, 0),
        ("Dangerous When Dead", 112332, 0, 0, 0),
        ("Red, Fraught, and Blue", 112332, 0, 0, 0),
        ("The Catch of the Siegfried", 112332, 0, 0, 0),
        ("The Copied Factory", 112375, 1, 2, 5),
        ("Onsal Hakair (Danshig Naadam)", 112376, 0, 0, 0),
        ("Anamnesis Anyder", 112378, 1, 1, 2),
        ("Eden's Verse: Fulmination", 112385, 2, 2, 4),
        ("Eden's Verse: Fulmination (Savage)", 112386, 2, 2, 4),
        ("Cinder Drift", 112379, 2, 2, 4),
        ("Cinder Drift (Extreme)", 112380, 2, 2, 4),
        ("Eden's Verse: Furor", 112387, 2, 2, 4),
        ("Eden's Verse: Furor (Savage)", 112388, 2, 2, 4),
        ("Ocean Fishing", 112383, 0, 0, 0),
        ("Memoria Misera (Extreme)", 112381, 2, 2, 4),
        ("Eden's Verse: Iconoclasm", 112389, 2, 2, 4),
        ("Eden's Verse: Iconoclasm (Savage)", 112390, 2, 2, 4),
        ("Eden's Verse: Refulgence", 112391, 2, 2, 4),
        ("Eden's Verse: Refulgence (Savage)", 112392, 2, 2, 4),
        ("The Bozjan Southern Front", 112401, 0, 0, 0),
        ("the Puppets' Bunker", 112400, 1, 2, 5),
        ("the Heroes' Gauntlet", 112399, 1, 1, 2),
        ("the Seat of Sacrifice", 112402, 2, 2, 4),
        ("the Seat of Sacrifice (Extreme)", 112403, 2, 2, 4),
        ("Matoya's Relict", 112406, 1, 1, 2),
        ("Eden's Promise: Litany", 112409, 2, 2, 4),
        ("Eden's Promise: Litany (Savage)", 112410, 2, 2, 4),
        ("Eden's Promise: Umbra", 112407, 2, 2, 4),
        ("Eden's Promise: Umbra (Savage)", 112408, 2, 2, 4),
        ("Eden's Promise: Anamorphosis", 112411, 2, 2, 4),
        ("Eden's Promise: Anamorphosis (Savage)", 112412, 2, 2, 4),
        ("Anything Gogo's", 112332, 0, 0, 0),
        ("Triple Triad Open Tournament", 112415, 0, 0, 0),
        ("Triple Triad Invitational Parlor", 112416, 0, 0, 0),
        ("Eden's Promise: Eternity", 112413, 2, 2, 4),
        ("Eden's Promise: Eternity (Savage)", 112414, 2, 2, 4),
        ("Delubrum Reginae", 112417, 0, 0, 0),
        ("Delubrum Reginae (Savage)", 112418, 0, 0, 0),
        ("Castrum Marinum", 112419, 2, 2, 4),
        ("Castrum Marinum (Extreme)", 112420, 2, 2, 4),
        ("Novice Mahjong (Quick Ranked Match)", 112336, 0, 0, 0),
        ("Advanced Mahjong (Quick Ranked Match)", 112336, 0, 0, 0),
        ("Four-player Mahjong (Quick Match, Kuitan Enabled)", 112336, 0, 0, 0),
        ("Four-player Mahjong (Quick Match, Kuitan Disabled)", 112336, 0, 0, 0),
        ("Paglth'an", 112428, 1, 1, 2),
        ("Zadnor", 112432, 0, 0, 0),
        ("The Tower at Paradigm's Breach", 112431, 1, 2, 5),
        ("The Cloud Deck", 112429, 2, 2, 4),
        ("The Cloud Deck (Extreme)", 112430, 2, 2, 4),
        ("The Tower of Zot", 112435, 1, 1, 2),
        ("the Stigma Dreamscape", 112442, 1, 1, 2),
        ("The Tower of Babil", 112436, 1, 1, 2),
        ("The Aitiascope", 112439, 1, 1, 2),
        ("Ktisis Hyperboreia", 112438, 1, 1, 2),
        ("Dragonsong's Reprise (Ultimate)", 112469, 2, 2, 4),
        ("Vanaspati", 112437, 1, 1, 2),
        ("The Mothercrystal", 112445, 2, 2, 4),
        ("The Minstrel's Ballad: Hydaelyn's Call", 112446, 2, 2, 4),
        ("The Dead Ends", 112440, 1, 1, 2),
        ("Smileton", 112441, 1, 1, 2),
        ("The Final Day", 112447, 2, 2, 4),
        ("The Phantoms' Feast", 112456, 0, 0, 0),
        ("Asphodelos: The Fourth Circle", 112454, 2, 2, 4),
        ("Asphodelos: The Fourth Circle (Savage)", 112455, 2, 2, 4),
        ("The Dark Inside", 112443, 2, 2, 4),
        ("The Minstrel's Ballad: Zodiark's Fall", 112444, 2, 2, 4),
        ("Asphodelos: The Third Circle", 112452, 2, 2, 4),
        ("Asphodelos: The Third Circle (Savage)", 112453, 2, 2, 4),
        ("Asphodelos: The First Circle", 112448, 2, 2, 4),
        ("Asphodelos: The First Circle (Savage)", 112449, 2, 2, 4),
        ("Asphodelos: The Second Circle", 112450, 2, 2, 4),
        ("Asphodelos: The Second Circle (Savage)", 112451, 2, 2, 4),
        ("the Aetherfont", 112521, 1, 1, 2),
        ("The Lunar Subterrane", 112543, 1, 1, 2),
        ("the Porta Decumana", 112468, 1, 1, 2),
        ("The Palaistra", 112472, 0, 0, 0),
        ("The Volcanic Heart", 112472, 0, 0, 0),
        ("Cloud Nine", 112472, 0, 0, 0),
        ("Alzadaal's Legacy", 112465, 1, 1, 2),
        ("The Minstrel's Ballad: Endsinger's Aria", 112467, 2, 2, 4),
        ("Crystalline Conflict (Custom Match - The Palaistra)", 112473, 0, 0, 0),
        ("Crystalline Conflict (Custom Match - The Volcanic Heart)", 112474, 0, 0, 0),
        ("Crystalline Conflict (Custom Match - Cloud Nine)", 112475, 0, 0, 0),
        ("Aglaia", 112466, 1, 2, 5),
        ("The Sil'dihn Subterrane", 112493, 0, 0, 0),
        ("the Fell Court of Troia", 112481, 1, 1, 2),
        ("Storm's Crown", 112482, 2, 2, 4),
        ("Storm's Crown (Extreme)", 112483, 2, 2, 4),
        ("Abyssos: The Fifth Circle", 112484, 2, 2, 4),
        ("Abyssos: The Fifth Circle (Savage)", 112485, 2, 2, 4),
        ("Abyssos: The Seventh Circle", 112488, 2, 2, 4),
        ("Abyssos: The Seventh Circle (Savage)", 112489, 2, 2, 4),
        ("Another Sil'dihn Subterrane", 112494, 1, 1, 2),
        ("Another Sil'dihn Subterrane (Savage)", 112495, 1, 1, 2),
        ("Abyssos: The Sixth Circle", 112486, 2, 2, 4),
        ("Abyssos: The Sixth Circle (Savage)", 112487, 2, 2, 4),
        ("Abyssos: The Eighth Circle", 112490, 2, 2, 4),
        ("Abyssos: The Eighth Circle (Savage)", 112491, 2, 2, 4),
        ("Mount Ordeals", 112503, 2, 2, 4),
        ("Lapis Manalis", 112502, 1, 1, 2),
        ("Eureka Orthos (Floors 1-10)", 112507, 0, 0, 0),
        ("Eureka Orthos (Floors 11-20)", 112508, 0, 0, 0),
        ("Eureka Orthos (Floors 21-30)", 112509, 0, 0, 0),
        ("Eureka Orthos (Floors 31-40)", 112510, 0, 0, 0),
        ("Eureka Orthos (Floors 41-50)", 112511, 0, 0, 0),
        ("Eureka Orthos (Floors 51-60)", 112512, 0, 0, 0),
        ("Eureka Orthos (Floors 61-70)", 112513, 0, 0, 0),
        ("Eureka Orthos (Floors 71-80)", 112514, 0, 0, 0),
        ("Eureka Orthos (Floors 81-90)", 112515, 0, 0, 0),
        ("Eureka Orthos (Floors 91-100)", 112516, 0, 0, 0),
        ("The Omega Protocol (Ultimate)", 112518, 2, 2, 4),
        ("Euphrosyne", 112505, 1, 2, 5),
        ("The Clockwork Castletown", 112472, 0, 0, 0),
        ("Crystalline Conflict (Custom Match - The Clockwork Castletown)", 112517, 0, 0, 0),
        ("Mount Ordeals (Extreme)", 112504, 2, 2, 4),
        ("Anabaseios: The Ninth Circle", 112524, 2, 2, 4),
        ("Anabaseios: The Ninth Circle (Savage)", 112525, 2, 2, 4),
        ("Anabaseios: The Tenth Circle", 112526, 2, 2, 4),
        ("Anabaseios: The Tenth Circle (Savage)", 112527, 2, 2, 4),
        ("Anabaseios: The Eleventh Circle", 112528, 2, 2, 4),
        ("Anabaseios: The Eleventh Circle (Savage)", 112529, 2, 2, 4),
        ("Anabaseios: The Twelfth Circle", 112530, 2, 2, 4),
        ("Anabaseios: The Twelfth Circle (Savage)", 112531, 2, 2, 4),
        ("Mount Rokkon", 112533, 0, 0, 0),
        ("Another Mount Rokkon", 112534, 1, 1, 2),
        ("Another Mount Rokkon (Savage)", 112535, 1, 1, 2),
        ("A Golden Opportunity", 112332, 0, 0, 0),
        ("The Voidcast Dais", 112522, 2, 2, 4),
        ("The Voidcast Dais (Extreme)", 112523, 2, 2, 4),
        ("Blunderville", 112552, 0, 0, 0),
        ("Aloalo Island", 112549, 0, 0, 0),
        ("Thaleia", 112546, 1, 2, 5),
        ("The Singularity Reactor (Unreal)", 112547, 2, 2, 4),
        ("The Abyssal Fracture", 112544, 2, 2, 4),
        ("The Abyssal Fracture (Extreme)", 112545, 2, 2, 4),
        ("The Red Sands", 112472, 0, 0, 0),
        ("Crystalline Conflict (Custom Match - The Red Sands)", 112548, 0, 0, 0),
        ("Another Aloalo Island", 112550, 1, 1, 2),
        ("Another Aloalo Island (Savage)", 112551, 1, 1, 2),
    ];
}