import React, { useEffect, useState } from 'react';
import './Monster.css';

function Monster() {
    const [monsters, setMonsters] = useState([]);
    const [monsterId, setSetMonsterId] = useState('');
    
    useEffect(() => {
        fetch('http://localhost:5236/api/Monster/GetAllMonsters')
            .then(response => response.json())
            .then(data => setMonsters(data));
    }, []);
    
    const handleDeleteMonster = () => {
        fetch(`http://localhost:5236/api/Monster/DeleteMonster?monsterId=${monsterId}`, {
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
    

    return (
        <div>
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
                            {monster.name}<br/>
                            {monster.zone}<br/>
                            {monster.type}<br/>
                            {monster.status}<br/>
                            Strength: {monster.attributes.strength}<br/>
                            Agility: {monster.attributes.agility}<br/>
                            Intelligence: {monster.attributes.intelligence}<br/>
                            Stamina: {monster.attributes.stamina}<br/>
                            Faith: {monster.attributes.faith}<br/>
                            Experience: {monster.attributes.experience}<br/>
                            Level: {monster.attributes.level}<br/>
                            {
                                /*
                                Possible Loot: {monster.possibleLoot.map(loot => (
                                <div key={loot.name}>
                                    Name: {loot.name}<br/>
                                    Type: {loot.type}<br/>
                                    Weight: {loot.weight}<br/>
                                    Dimensions: {loot.dimensions}<br/>
                                    Value: {loot.value}<br/>
                                </div>
                                */
                            }
                        </button>
                    ))}
                </div>
            <h1 className='i-m'>Admin-Delete a monster</h1>
            <div className='delete-monster'>
                <input type="number" placeholder="Enter monster ID" value={monsterId} onChange={e => setSetMonsterId(e.target.value)} />
                <button onClick={handleDeleteMonster}>Delete a Monster</button>
            </div>
        </div>
    );
}

export default Monster;
