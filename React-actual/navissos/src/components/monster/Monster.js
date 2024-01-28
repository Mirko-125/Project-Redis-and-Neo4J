import React, { useEffect, useState } from 'react';
import './Monster.css';

function Monster() {
    const [monsters, setMonsters] = useState([]);
    const [monsterId, setSetMonsterId] = useState('');
    const [monsterName, setMonsterName] = useState('');
    const [monsterZone, setMonsterZone] = useState('');
    const [monsterType, setMonsterType] = useState('');
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
    
    const handleCreateMonster = () => {
        const monsterData = {
            name: monsterName,
            zone: monsterZone,
            type: monsterType,
            imageURL: monsterImageURL,
            status: monsterStatus,
            attributes: {
                strength: monsterStrength,
                agility: monsterAgility,
                intelligence: monsterIntelligence,
                stamina: monsterStamina,
                faith: monsterFaith,
                experience: monsterExperience,
                level: monsterLevel
            },
            possibleLoot: possibleLootNames.map(lootName => ({ name: lootName }))
        };

        fetch('http://localhost:5236/api/NPC/AddNPC', {
            method: 'POST',
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

    useEffect(() => {
        fetch('http://localhost:5236/api/Monster/GetAll')
            .then(response => response.json())
            .then(data => setMonsters(data));
    }, []);
    
    const handleDeleteMonster = () => {
        fetch(`http://localhost:5236/api/Monster?monsterId=${monsterId}`, {
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
            monsterId: monsterId,
            zone: monsterZone,
            imageURL: monsterImageURL,
            attributes: {
                strength: monsterStrength,
                agility: monsterAgility,
                intelligence: monsterIntelligence,
                stamina: monsterStamina,
                faith: monsterFaith,
                experience: monsterExperience,
                level: monsterLevel
            },
            status: monsterStatus
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


    return (
        <div className='dungeon-wrap'>
            <h1 className='i-m'>Monsters</h1>
                <div className="monster-horde">
                    {monsters.map(monster => (
                        <button key={monster.id} 
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
            <h1 className='i-m'>Admin-Make a monster</h1>
            <div className='create-monster'>
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster name"
                    onChange={(e) => setMonsterName(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster zone"
                    onChange={(e) => setMonsterZone(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster type"
                    onChange={(e) => setMonsterType(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster image URL"
                    onChange={(e) => setMonsterImageURL(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster status"
                    onChange={(e) => setMonsterStatus(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster strength"
                    onChange={(e) => setMonsterStrength(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster agility"
                    onChange={(e) => setMonsterAgility(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster intelligence"
                    onChange={(e) => setMonsterIntelligence(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster stamina"
                    onChange={(e) => setMonsterStamina(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster faith"
                    onChange={(e) => setMonsterFaith(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster experience"
                    onChange={(e) => setMonsterExperience(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster level"
                    onChange={(e) => setMonsterLevel(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter possible loot names (separated by commas)"
                    onChange={(e) => setPossibleLootNames(e.target.value.split(','))}
                />
                <button onClick={handleCreateMonster}>Make a monster</button>
            </div>
            <h1 className='i-m'>Admin: Update a monster</h1>
            <div className='update-monster'>
                <input 
                    className='create-input' 
                    type="number" 
                    placeholder="Enter monster ID" 
                    value={monsterId} 
                    onChange={e => setSetMonsterId(e.target.value)} 
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster name"
                    onChange={(e) => setMonsterName(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster zone"
                    onChange={(e) => setMonsterZone(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster type"
                    onChange={(e) => setMonsterType(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster image URL"
                    onChange={(e) => setMonsterImageURL(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster status"
                    onChange={(e) => setMonsterStatus(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster strength"
                    onChange={(e) => setMonsterStrength(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster agility"
                    onChange={(e) => setMonsterAgility(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster intelligence"
                    onChange={(e) => setMonsterIntelligence(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster stamina"
                    onChange={(e) => setMonsterStamina(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster faith"
                    onChange={(e) => setMonsterFaith(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster experience"
                    onChange={(e) => setMonsterExperience(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter monster level"
                    onChange={(e) => setMonsterLevel(e.target.value)}
                />
                <button onClick={handleUpdateMonster}>Update monster</button>
            </div>
            <h1 className='i-m'>Admin-Delete a monster</h1>
            <div className='delete-monster'>
                <input type="number" placeholder="Enter monster ID" value={monsterId} onChange={e => setSetMonsterId(e.target.value)} />
                <button onClick={handleDeleteMonster}>Delete a monster</button>
            </div>
        </div>
    );
}

export default Monster;
