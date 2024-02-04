import React, { useEffect, useState } from 'react';
import './Monster.css';
import '../../styling/CrudContainer.css';


function Monster() {
    const [monsters, setMonsters] = useState([]);
    const [monsterBattleId, setBattleId] = useState('');
    const [monsterName, setMonsterName] = useState('');
    const [playerName, setPlayerName] = useState('');
    const [monsterOldName, setMonsterOldName] = useState('');
    const [monsterZone, setMonsterZone] = useState('');
    const [monsterType, setMonsterType] = useState('');
    const [lootItemsNames, setLoot] = useState([]);
    const [monsterImageURL, setMonsterImageURL] = useState('');
    const [monsterStatus, setMonsterStatus] = useState('');
    const [monsterStrength, setMonsterStrength] = useState('');
    const [monsterAgility, setMonsterAgility] = useState('');
    const [monsterIntelligence, setMonsterIntelligence] = useState('');
    const [monsterStamina, setMonsterStamina] = useState('');
    const [monsterFaith, setMonsterFaith] = useState('');
    const [monsterExperience, setMonsterExperience] = useState('');
    const [monsterLevel, setMonsterLevel] = useState('');
    const [possibleLootNames, setPossibleLootNames] = useState([]);
    const [singleMonster, setSingleMonster] = useState([]);
    
    const handleFindMonster = () => {
        return fetch(`http://localhost:5236/api/Monster/GetOne?monsterName=${monsterName}`)
            .then(response => response.json())
            .then(data => {
                setSingleMonster(data);
                return data;
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    const handleFinalizeBattle = () => {
        const finalizeBattleData = {
            monsterBattleId,
            lootItemsNames
        };
        console.log(finalizeBattleData);
        const finalizeBattleDataJson = JSON.stringify(finalizeBattleData);
        return fetch(`http://localhost:5236/api/MonsterBattle/Finalize`, {
            method: 'PUT',
            headers: {
                'Content-type': 'application/json'
            },
            body: finalizeBattleDataJson
        })  .then(response => response.json())
            .then(data => {
                console.log(data);
            }).catch(error => {
                console.error(error);
            });
    }

    const handleBattleMonster = () => {
        const monsterFight = {
            monsterName,
            playerName,
        }
        
        const monsterData = JSON.stringify(monsterFight);
        console.log(monsterFight);

        fetch(`http://localhost:5236/api/MonsterBattle?monsterName=${monsterName}&playerName=${playerName}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: monsterData
        })
            .then(response => response.json())
            .then(data => {
                console.log(data);
            }).catch(error => {
                console.error(error);
            });
    }

    const handleCreateMonster = () => {
        const monsterData = {
            name: monsterName,
            zone: monsterZone,
            type: monsterType,
            imageURL: monsterImageURL,
            status: monsterStatus,
            attributes: 
            {
                strength: monsterStrength,
                agility: monsterAgility,
                intelligence: monsterIntelligence,
                stamina: monsterStamina,
                faith: monsterFaith,
                experience: monsterExperience,
                level: monsterLevel
            },
            possibleLootNames: possibleLootNames
        };
        const data = JSON.stringify(monsterData);
        console.log(data);
        fetch('http://localhost:5236/api/Monster', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: data
        })
            .then(response => response.json())
            .then(data => {
                console.log(data);
                window.location.reload();
                return data;
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    useEffect(() => {
        fetch('http://localhost:5236/api/Monster/GetAll')
            .then(response => response.json())
            .then(data => setMonsters(data));
    }, []);
    
    const handleDeleteMonster = () => {
        fetch(`http://localhost:5236/api/Monster?monsterName=${monsterName}`, {
            method: 'DELETE'
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };
    
    const handleUpdateMonster = () => {
        const monsterData = {
            name: monsterName,
            zone: monsterZone,
            imageURL: monsterImageURL,
            type: monsterType,
            attributes: {
                strength: monsterStrength,
                agility: monsterAgility,
                intelligence: monsterIntelligence,
                stamina: monsterStamina,
                faith: monsterFaith,
                experience: monsterExperience,
                level: monsterLevel
            },
            status: monsterStatus,
            oldName: monsterOldName
        };

        fetch('http://localhost:5236/api/Monster/Update', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(monsterData)
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    const printMonster = (monster) => {
        setMonsterName(monster.name);
        setMonsterZone(monster.zone);
        setMonsterType(monster.type);
        setMonsterImageURL(monster.imageURL);
        setMonsterStatus(monster.status);
        setMonsterStrength(monster.attributes.strength);
        setMonsterAgility(monster.attributes.agility);
        setMonsterIntelligence(monster.attributes.intelligence);
        setMonsterStamina(monster.attributes.stamina);
        setMonsterFaith(monster.attributes.faith);
        setMonsterExperience(monster.attributes.experience);
        setMonsterLevel(monster.attributes.level);
    } 


    return (
        <div className='dungeon-wrap'>
            <h1 className='i-m'>Monsters</h1>
                <div className="monster-horde">
                    {monsters.map(monster => (
                        <button key={monster.id} onClick={() => printMonster(monster)}
                        style={{
                            backgroundImage: `url(${monster.imageURL})`,
                            backgroundSize: 'cover',
                            width: '200px', // adjust to the size of your image
                            height: '100px', // adjust to the size of your image
                            border: 'none',
                            cursor: 'pointer',
                            margin: '3rem'
                        }}
                        >
                        </button>
                    ))}
                </div>
            <div className='crud-container'>
                <div className='crud-item'>
                    <h1 className='i-m'>Admin-Make a monster</h1>
                        <div className='input-container'>
                            <input
                                type="text"
                                placeholder="Enter monster name"
                                value={monsterName}
                                onChange={(e) => setMonsterName(e.target.value)}
                            />
                            <input
                                type="text"
                                placeholder="Enter monster zone"
                                value={monsterZone}
                                onChange={(e) => setMonsterZone(e.target.value)}
                            />
                            <input
                                type="text"
                                placeholder="Enter monster type"
                                value={monsterType}
                                onChange={(e) => setMonsterType(e.target.value)}
                            />
                            <input
                                type="text"
                                placeholder="Enter monster image URL"
                                value={monsterImageURL}
                                onChange={(e) => setMonsterImageURL(e.target.value)}
                            />
                            <input
                                type="text"
                                placeholder="Enter monster status"
                                value={monsterStatus}
                                onChange={(e) => setMonsterStatus(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster strength"
                                value={monsterStrength}
                                onChange={(e) => setMonsterStrength(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster agility"
                                value={monsterAgility}
                                onChange={(e) => setMonsterAgility(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster intelligence"
                                value={monsterIntelligence}
                                onChange={(e) => setMonsterIntelligence(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster stamina"
                                value={monsterStamina}
                                onChange={(e) => setMonsterStamina(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster faith"
                                value={monsterFaith}
                                onChange={(e) => setMonsterFaith(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster experience"
                                value={monsterExperience}
                                onChange={(e) => setMonsterExperience(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster level"
                                value={monsterLevel}
                                onChange={(e) => setMonsterLevel(e.target.value)}
                            />
                            <input
                                type="text"
                                placeholder="Enter possible loot names (separated by commas)"
                                onChange={(e) => setPossibleLootNames(e.target.value.split(','))}
                            />
                            <button className='blue-bg' onClick={handleCreateMonster}>Make a monster</button>
                        </div>
                    </div>
                    <div className='crud-item'>
                        <h1 className='i-m'>Admin: Update a monster</h1>
                        <div className='input-container'>
                        <input
                                type="text"
                                placeholder="Enter monster name"
                                value={monsterName}
                                onChange={(e) => setMonsterName(e.target.value)}
                            />
                            <input
                                type="text"
                                placeholder="Enter monster zone"
                                value={monsterZone}
                                onChange={(e) => setMonsterZone(e.target.value)}
                            />
                            <input
                                type="text"
                                placeholder="Enter monster type"
                                value={monsterType}
                                onChange={(e) => setMonsterType(e.target.value)}
                            />
                            <input
                                type="text"
                                placeholder="Enter monster image URL"
                                value={monsterImageURL}
                                onChange={(e) => setMonsterImageURL(e.target.value)}
                            />
                            <input
                                type="text"
                                placeholder="Enter monster status"
                                value={monsterStatus}
                                onChange={(e) => setMonsterStatus(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster strength"
                                value={monsterStrength}
                                onChange={(e) => setMonsterStrength(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster agility"
                                value={monsterAgility}
                                onChange={(e) => setMonsterAgility(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster intelligence"
                                value={monsterIntelligence}
                                onChange={(e) => setMonsterIntelligence(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster stamina"
                                value={monsterStamina}
                                onChange={(e) => setMonsterStamina(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster faith"
                                value={monsterFaith}
                                onChange={(e) => setMonsterFaith(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster experience"
                                value={monsterExperience}
                                onChange={(e) => setMonsterExperience(e.target.value)}
                            />
                            <input
                                type="number"
                                placeholder="Enter monster level"
                                value={monsterLevel}
                                onChange={(e) => setMonsterLevel(e.target.value)}
                            />
                            <input
                                type="text"
                                placeholder="Current name"
                                onChange={(e) => setMonsterOldName(e.target.value)}
                            />
                            <button className='violet-bg' onClick={handleUpdateMonster}>Update monster</button>
                        </div>
                </div>
                <div className='crud-item'>
                    <h1 className='i-m'>Admin-Delete a monster</h1>
                    <div className='input-container'>
                        <input type="text" placeholder="Enter monster's name" value={monsterName} onChange={e => setMonsterName(e.target.value)} />
                        <button className='red-bg' onClick={handleDeleteMonster}>Delete a monster</button>
                    </div>
                </div>
                <div className='crud-item'>
                    <h1 className='i-p'>Gamewise: See a the certain Monster</h1>
                    <div className='input-container'>
                        <input type="text" placeholder="Monster's name is..." value={monsterName} onChange={e => setMonsterName(e.target.value)} />
                        <button className='green-bg' onClick={handleFindMonster}>Call</button>
                        <div className='players'>
                            <div style={{
                                backgroundColor: 'black',
                                backgroundSize: 'cover',
                                width: '200px',
                                height: '200px', 
                                border: 'none',
                                cursor: 'pointer',
                                margin: '3rem',
                                color: 'white'
                            }}
                            >
                                Name: {singleMonster.name}<br/>
                                Zone: {singleMonster.zone}<br/>
                                Type: {singleMonster.type}<br/>
                                Image source: {singleMonster.imageURL}<br/>
                                Status: {singleMonster.status}<br/>
                        </div>
                    </div>
                    </div>
                </div>
                <div className='crud-item'>
                    <h1 className='i-p'>Battle a monster!</h1>
                    <div className='input-container'>
                    <input 
                        type="text" 
                        placeholder="Monster's name is..." 
                        onChange={e => setMonsterName(e.target.value)}
                    />
                    <input 
                        type="text" 
                        placeholder="Player's name is..." 
                        onChange={e => setPlayerName(e.target.value)}
                    />
                    <button className='cream-bg' onClick={handleBattleMonster}>Battle!</button>
                    </div>
                </div>
                <div className='crud-item'>
                    <h1 className='i-p'>Finalize Battle!</h1>
                    <div className='input-container'>
                    <input 
                        type="number" 
                        placeholder="BattleID" 
                        onChange={e => setBattleId(e.target.value)}
                    />
                    <input
                        type="text"
                        placeholder="Enter item names (separated by commas)"
                        onChange={(e) => setLoot(e.target.value.split(','))}
                    />
                    <button className='violet-bg' onClick={handleFinalizeBattle}>Finalize!</button>
                    </div>
                </div>
        </div>
    </div>
    );
}

export default Monster;
