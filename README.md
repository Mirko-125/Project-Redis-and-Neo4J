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
konacan i stabilan repozitorijum je na grani main i nad commitom finished databases

baze podataka:
Neo4J - pokrenut preko dokera, pristup podacima bazi preko neo4j browser-a: http://localhost:7474/browser/
Redis - pokrenut preko dokera.

Neo4J je koriscen za podatke dok Redis za kesiranje, u prilogu je startni neo4j upit (cisto unapred da bi postajali neki entiteti):
//skripta

dodatne tehnologije:
c# dotnet, pokrenuti Database-Acces u terminalu i pokrenuti backend komandom dotnet watch run
react, preko npm-a pokrenuti React-actual/navissos u terminalu i pokrenuti front komandom npm start

napomena:
posto nismo stigli da zavrsimo front-end, molimo ako nije problem da se stvari koje nisu realizovane da se istestiraju u swaggeru.
