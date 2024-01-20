# Project-Redis-and-Neo4J
Project for advanced database systems using Redis and Neo4J

Technologies used:

# Redis
https://redis.io/

# Neo4J

https://neo4j.com/

# The Elder Scrolls: Navissos

The Elder Scrolls: Navissos - "The Elder Scrolls" is an MMORPG game that in its latest edition "Navissos" uses Neo4J and Redis databases. Neo4J is used for the very data structure and connections in the game such as the players, characters, their classes, territories, achievements, equipment and others. Redis is used to cache dialogue between characters during gameplay and any transactions that don't need to be done immediately, the virtual store will be shared between all players, so it will be constantly cached for faster interactions.

# About the project

Mirko Bojanic 18087, Stefana Miladinovic 18251, Jelena Mladenovic 18295, Milos Miljkovic 19040. Projekat: Redis + Neo4J

github repo:
https://github.com/Mirko-125/Project-Redis-and-Neo4J
konacan i stabilan repozitorijum je na grani main i nad commitom graph databases

baze podataka:
Neo4J - pokrenut preko dokera, pristup podacima bazi preko neo4j browser-a: http://localhost:7474/browser/
Redis - pokrenut preko dokera.

Neo4J je koriscen za podatke dok Redis za kesiranje, u prilogu je startni neo4j upit (cisto unapred da bi postajali neki entiteti):

CREATE (class1:Class { name: 'Soldier'})
CREATE (base1:Attributes { strength: 76, agility: 89, intelligence: 63, stamina: 80, faith: 80, experience: 50, level: 6})
CREATE (level1:Attributes { strength: 78, agility: 90, intelligence: 65, stamina: 82, faith: 82, experience: 52, level: 6})
CREATE (class1)-[:HAS_BASE_ATTRIBUTES]->(base1)
CREATE (class1)-[:LEVEL_GAINS_ATTRIBUTES]->(level1)

CREATE (class2:Class { name: 'General'})
CREATE (base2:Attributes { strength: 80, agility: 80, intelligence: 78, stamina: 76, faith: 85, experience: 67, level: 7})
CREATE (level2:Attributes { strength: 85, agility: 81, intelligence: 80, stamina: 74, faith: 82, experience: 68, level: 7})
CREATE (class2)-[:HAS_BASE_ATTRIBUTES]->(base2)
CREATE (class2)-[:LEVEL_GAINS_ATTRIBUTES]->(level2)

CREATE (class3:Class { name: 'Mage'})
CREATE (base3:Attributes { strength: 88, agility: 44, intelligence: 90, stamina: 54, faith: 68, experience: 77, level: 8})
CREATE (level3:Attributes { strength: 89, agility: 50, intelligence: 91, stamina: 57, faith: 70, experience: 80, level: 8})
CREATE (class3)-[:HAS_BASE_ATTRIBUTES]->(base3)
CREATE (class3)-[:LEVEL_GAINS_ATTRIBUTES]->(level3)


CREATE (class4:Class { name: 'Roman'})
CREATE (base4:Attributes { strength: 58, agility: 73, intelligence: 69, stamina: 78, faith: 80, experience: 47, level: 7})
CREATE (level4:Attributes { strength: 59, agility: 78, intelligence: 74, stamina: 87, faith: 81, experience: 50, level: 7})
CREATE (class4)-[:HAS_BASE_ATTRIBUTES]->(base4)
CREATE (class4)-[:LEVEL_GAINS_ATTRIBUTES]->(level4)


CREATE (class5:Class { name: 'Technition'})
CREATE (base5:Attributes { strength: 48, agility: 63, intelligence: 59, stamina: 68, faith: 50, experience: 37, level: 3})
CREATE (level5:Attributes { strength: 49, agility: 63, intelligence: 74, stamina: 70, faith: 51, experience: 39, level: 3})
CREATE (class5)-[:HAS_BASE_ATTRIBUTES]->(base5)
CREATE (class5)-[:LEVEL_GAINS_ATTRIBUTES]->(level5)

	
# Da bi i druga skripta proradila, treba rucno proslediti id klase


CREATE (n1:Player { name: 'user13', email: 'rastko44@gmail.com', bio: 'HAHAHA', achievementPoints: 5, createdAt: 'mart 2020', password: '27013003', gold: 30043, honor: 4323})
WITH n1
//NEOPHODNO UNETI NOVI ODGOVARAJUCI ID KLASE
MATCH (class6:Class) WHERE ID(class6)=32
CREATE (n1)-[:IS]->(class6)
WITH n1, class6
MATCH (class6)-[:HAS_BASE_ATTRIBUTES]->(x1)
CREATE (m1:Inventory {weightLimit : 170, dimensions: 2, freeSpots: 4, usedSpots: 2})
CREATE (o1:Attributes { strength: x1.strength, agility: x1.agility, intelligence: x1.intelligence, stamina: x1.stamina, faith: x1.faith, experience: 4, level: 5})
CREATE (p1:Equipment { averageQuality: 67, weight: 100})
CREATE (n1)-[:OWNS]->(m1)
CREATE (n1)-[:HAS]->(o1)
CREATE (n1)-[:WEARS]->(p1)

CREATE (n2:Player { name: 'nekotamo', email: 'ostalo4@gmail.com', bio: 'Bice bolje', achievementPoints: 5, createdAt: 'septembar 2021', password: '36575863', gold: 2546, honor: 2234})
WITH n2
//NEOPHODNO UNETI NOVI ODGOVARAJUCI ID KLASE
MATCH (class7:Class) WHERE ID(class7)=35
CREATE (n2)-[:IS]->(class7)
WITH n2, class7
MATCH (class7)-[:HAS_BASE_ATTRIBUTES]->(x2)
CREATE (m2:Inventory {weightLimit : 110, dimensions: 3, freeSpots: 3, usedSpots: 3})
CREATE (o2:Attributes { strength: x2.strength, agility: x2.agility, intelligence: x2.intelligence, stamina: x2.stamina, faith: x2.faith, experience: 5, level: 6})
CREATE (p2:Equipment { averageQuality: 63, weight: 99})
CREATE (n2)-[:OWNS]->(m2)
CREATE (n2)-[:HAS]->(o2)
CREATE (n2)-[:WEARS]->(p2)

CREATE (n3:Player { name: 'user45', email: 'dada7@gmail.com', bio: 'blabla', achievementPoints: 3, createdAt: 'oktobar 2022', password: '66429863', gold: 1576, honor: 3334})
WITH n3
//NEOPHODNO UNETI NOVI ODGOVARAJUCI ID KLASE
MATCH (class8:Class) WHERE ID(class8)=38
CREATE (n3)-[:IS]->(class8)
WITH n3, class8
MATCH (class8)-[:HAS_BASE_ATTRIBUTES]->(x3)
CREATE (m3:Inventory {weightLimit : 115, dimensions: 3, freeSpots: 2, usedSpots: 4})
CREATE (o3:Attributes { strength: x3.strength, agility: x3.agility, intelligence: x3.intelligence, stamina: x3.stamina, faith: x3.faith, experience: 6, level:7})
CREATE (p3:Equipment { averageQuality: 70, weight: 100})
CREATE (n3)-[:OWNS]->(m3)
CREATE (n3)-[:HAS]->(o3)
CREATE (n3)-[:WEARS]->(p3)


CREATE (consumable1:Consumable:Item {
                            name: 'Jabuka',
                            weight: 5,
                            type: 'Voce',
                            dimensions: 1,
                            value: 5,
                            effect:'Osvezenje'
                           })


CREATE (consumable2:Consumable:Item {
                            name: 'Cokolada',
                            weight: 3,
                            type: 'Slatkis',
                            dimensions: 3,
                            value: 8,
                            effect:'Energije++'
                            })

CREATE (consumable3:Consumable:Item {
                            name: 'Brokoli',
                            weight: 8,
                            type: 'Povrce',
                            dimensions: 5,
                            value: 8,
                            effect:'Snaga++'
                            })


CREATE (gear1:Item:Gear {
                        name: 'Mac',
                        weight: 10,
                        type: 'Oruzje',
                        dimensions: 10,
                        value: 20,
                        slot: 1,
                        level: 7,
                        quality: 'Legendary'
                        })
                        CREATE (attributes1:Attributes { 
                            strength: 90, 
                            agility: 88, 
                            intelligence: 0, 
                            stamina: 80, 
                            faith: 70, 
                            experience: 72, 
                            levelAttributes: 7
                        })
                        CREATE (gear)-[:HAS]->(attributes)


CREATE (gear2:Item:Gear {
                        name: 'Kamen',
                        weight: 3,
                        type: 'Oruzje',
                        dimensions: 5,
                        value: 3,
                        slot: 2,
                        level: 1,
                        quality: 'Regular'
                        })
                        CREATE (attributes2:Attributes { 
                            strength: 50, 
                            agility: 40, 
                            intelligence: 0, 
                            stamina: 41, 
                            faith: 30, 
                            experience: 28, 
                            levelAttributes: 1
                        })
                        CREATE (gear)-[:HAS]->(attributes)

CREATE (gear3:Item:Gear {
                        name: 'Sablja',
                        weight: 12,
                        type: 'Oruzje',
                        dimensions: 10,
                        value: 12,
                        slot: 3,
                        level: 9,
                        quality: 'Epic'
                        })
                        CREATE (attributes3:Attributes { 
                            strength: 82, 
                            agility: 80, 
                            intelligence: 0, 
                            stamina: 70, 
                            faith: 76, 
                            experience: 68, 
                            levelAttributes: 9
                        })
                        CREATE (gear)-[:HAS]->(attributes)


CREATE (marketplace1:Marketplace {
                       zone: 'Sever',
                       itemCount: 50,
                       restockCycle: 10
		      })

CREATE (marketplace2:Marketplace {
                       zone: 'Jug',
                       itemCount: 40,
                       restockCycle: 8
		      })

CREATE (marketplace3:Marketplace {
                       zone: 'Istok',
                       itemCount: 60,
                       restockCycle: 9
		      })

CREATE (marketplace4:Marketplace {
                       zone: 'Zapad',
                       itemCount: 30,
                       restockCycle: 12
		      })

CREATE (a:Ability { name: "Ice Shard", damage: 30, cooldown: 8, range: 20, special: "slow effect", heal: 0 })

CREATE (b:Ability { name: "Healing Touch", damage: 0, cooldown: 15, range: 5, special: "none", heal: 50 })

CREATE (c:Ability { name: "Thunder Strike", damage: 40, cooldown: 12, range: 15, special: "stun effect", heal: 0 })

CREATE (d:Ability { name: "Earthquake", damage: 45, cooldown: 20, range: 25, special: "area damage", heal: 0 })

CREATE (e:Ability { name: "Wind Gust", damage: 25, cooldown: 5, range: 30, special: "knockback effect", heal: 0 })

CREATE (f:Ability { name: "Water Surge", damage: 35, cooldown: 10, range: 15, special: "push effect", heal: 0 })

CREATE (g:Ability { name: "Shadow Veil", damage: 0, cooldown: 30, range: 0, special: "invisibility", heal: 0 })

CREATE (h:Ability { name: "Light Beam", damage: 50, cooldown: 15, range: 20, special: "blind effect", heal: 0 })

CREATE (i:Ability { name: "Nature's Blessing", damage: 0, cooldown: 60, range: 0, special: "none", heal: 100 })

CREATE (n:Ability { name: "Fireball", 
                            damage: 35,
                            cooldown: 10,
                            range: 15,
                            special: "multiple fires",
                            heal: 0
                        })
CREATE (a1:Achievement { name: "First Blood", type: "Optional", points: 10, conditions: "Make the first kill in a game" })

CREATE (b1:Achievement { name: "Survivor", type: "Mandatory", points: 20, conditions: "Survive for 24 hours in the game" })

CREATE (c1:Achievement { name: "Treasure Hunter", type: "Optional", points: 30, conditions: "Find a hidden treasure" })

CREATE (d1:Achievement { name: "Master Crafter", type: "Optional", points: 40, conditions: "Craft 100 items" })

CREATE (e1:Achievement { name: "Monster Slayer", type: "Mandatory", points: 50, conditions: "Kill 1000 monsters" })

CREATE (f1:Achievement { name: "Explorer", type: "Optional", points: 60, conditions: "Discover all locations on the map" })

CREATE (g1:Achievement { name: "Champion", type: "Mandatory", points: 70, conditions: "Win a PvP tournament" })

CREATE (h1:Achievement { name: "Guild Leader", type: "Optional", points: 80, conditions: "Create and lead a guild with 50 members" })

CREATE (i1:Achievement { name: "Legend", type: "Mandatory", points: 100, conditions: "Reach the maximum level" })

CREATE (n1:Achievement {
                            name: "Best friends forever",
                            type: "Mandatory",
                            points: 35,
                            conditions: "Work on an elfak project together"
                        })

//MARKETPLACE

CREATE (marketplace12:Marketplace {
                       zone: 'Sever',
                       itemCount: 50,
                       restockCycle: 10
		      })

CREATE (marketplace22:Marketplace {
                       zone: 'Jug',
                       itemCount: 40,
                       restockCycle: 8
		      })

CREATE (marketplace32:Marketplace {
                       zone: 'Istok',
                       itemCount: 60,
                       restockCycle: 9
		      })

CREATE (marketplace42:Marketplace {
                       zone: 'Zapad',
                       itemCount: 30,
                       restockCycle: 12
		      })


//MONSTER

CREATE (monster1:Monster {
    name: 'Vestica',
    zone: 'Jug',
    type: 'Nepobediva',
    imageURL: 'https://static.wikia.nocookie.net/enterthegungeon_gamepedia/images/4/4a/Confirmed.png',
    status: 'Nepobediva'
})
CREATE (monsterAttributes1:Attributes {
    strength: 69,
    agility: 69,
    intelligence: 80,
    stamina: 70,
    faith: 72,
    experience: 75,
    level: 6
})
CREATE (monster1)-[:HAS]->(monsterAttributes1)
CREATE (monster1)-[:POSSIBLE_LOOT]->(itemMonster1:Item {name: 'Sablja'})


CREATE (monster2:Monster {
    name: 'Redistrator',
    zone: 'Sever',
    type: 'Neunistiv',
    imageURL: 'https://static.wikia.nocookie.net/enterthegungeon_gamepedia/images/a/a4/Bloodbulon.png',
    status: 'Neunistiv'
})
CREATE (monsterAttributes2:Attributes {
    strength: 70,
    agility: 59,
    intelligence: 69,
    stamina: 55,
    faith: 80,
    experience: 77,
    level: 7
})
CREATE (monster2)-[:HAS]->(monsterAttributes2)
CREATE (monster2)-[:POSSIBLE_LOOT]->(itemMonster2:Item {name: 'Mac'})

CREATE (monster3:Monster {
    name: 'CassandraDokumentacija',
    zone: 'Zapad',
    type: 'Optional',
    imageURL: 'https://static.wikia.nocookie.net/enterthegungeon_gamepedia/images/1/15/Blue_Bookllet.png',
    status: 'Optional'
})
CREATE (monsterAttributes3:Attributes {
    strength: 88,
    agility: 70,
    intelligence: 65,
    stamina: 78,
    faith: 70,
    experience: 66,
    level: 5
})
CREATE (monster3)-[:HAS]->(monsterAttributes3)
CREATE (monster3)-[:POSSIBLE_LOOT]->(itemMonster3:Item {name: 'Kamen'})


//NPC


CREATE (npc1:NPC {
                name: "Ucitelj",
                affinity: 'Pazljiv',
                imageURL: 'https://static.wikia.nocookie.net/enterthegungeon_gamepedia/images/2/23/Ser_Manuel.png',
                zone: 'Sever',
                mood: 'Dostupan'
               })

CREATE (npc2:NPC {
                name: "Pripovedac",
                affinity: 'Veran',
                imageURL: 'https://static.wikia.nocookie.net/enterthegungeon_gamepedia/images/9/90/Gunsling_King.png',
                zone: 'Zapad',
                mood: 'Dostupan'
               })

CREATE (npc3:NPC {
    name: "Savetnik",
    affinity: 'Tajanstven',
    imageURL: 'https://static.wikia.nocookie.net/enterthegungeon_gamepedia/images/a/a8/Manservantes.png',
    zone: 'Istok',
    mood: 'Zamisljen'
})

dodatne tehnologije:
c# dotnet, pokrenuti Database-Acces u terminalu i pokrenuti backend komandom dotnet watch run
react, preko npm-a pokrenuti React-actual/navissos u terminalu i pokrenuti front komandom npm start

# napomena:
posto nismo stigli da zavrsimo front-end, molimo ako nije problem da se stvari koje nisu realizovane da se istestiraju u swaggeru.
