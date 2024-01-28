import React, { useEffect, useState } from 'react';
import './Ability.css';

const Ability = () => {
    const [abilityId, setAbilityId] = useState('');
    const [playerId, setPlayerId] = useState('');
    const [ability, setAbilities] = useState([]);
    const [name, setName] = useState('');
    const [damage, setDamage] = useState(0);
    const [cooldown, setCooldown] = useState(0);
    const [range, setRange] = useState(0);
    const [special, setSpecial] = useState('');
    const [heal, setHeal] = useState(0);

    const handleCreateAbility = () => {
        const abilityData = {
            name: name,
            damage: damage,
            cooldown: cooldown,
            range: range,
            special: special,
            heal: heal
        };

        fetch('localhost:5236/api/Ability/CreateAbility', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(abilityData)
        })
        .then(response => {
            if (response.ok) {
                
            } else {

            }
        })
        .catch(error => {

        });
    };

    const handleUpdateAbility = () => {
        const abilityData = {
            id: abilityId,
            name: name,
            damage: damage,
            cooldown: cooldown,
            range: range,
            special: special,
            heal: heal
        };

        fetch('http://localhost:5236/api/Ability/UpdateAbility', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(abilityData)
        })
        .then(response => {
            if (response.ok) {
                
            } else {

            }
        })
        .catch(error => {

        });
    };

    const handleDeleteAbility = () => {
        fetch(`http://localhost:5236/api/Ability?abilityId=${abilityId}`, {
            method: 'DELETE'
        })
            .then(response => response.json())
            .then(data => {
           
                console.log(data);
            })
            .catch(error => {
            
                console.error(error);
            });
    };

    const handleAssignAbility = () => {
        fetch(`http://localhost:5236/api/Ability/AssignAbility?abilityId=${abilityId}&playerId=${playerId}`, {
            method: 'POST'
        })
            .then(response => response.json())
            .then(data => {
           
                console.log(data);
            })
            .catch(error => {
            
                console.error(error);
            });
    };

    useEffect(() => {
        fetch('http://localhost:5236/api/Ability/AllAbilites')
            .then(response => response.json())
            .then(data => setAbilities(data));
    }, []);
    return (
        <div>
            <h1 className='i-abs'>All abilites</h1>
            <div className='people'>
            {ability.map(ability => (
                <button key={ability.id} style={{
                    backgroundColor: '#301934',
                    backgroundSize: 'cover',
                    width: '200px',
                    height: '125px', 
                    border: 'none',
                    cursor: 'pointer',
                    margin: '3rem',
                    color: 'white'
                }}>
                    Id: [{ability.id}]<br/>
                    {ability.properties.name}<br/>
                    Special: {ability.properties.special}<br/>
                    Damage: {ability.properties.damage}<br/>
                    Heal: {ability.properties.heal}<br/>
                    Cooldown: {ability.properties.cooldown}<br/>
                    Range: {ability.properties.range}<br/>

                </button>
            ))}
            </div>
            <h1 className='i-a'>Create an ability</h1>
            <div className='create-ability'>
                <input
                    className='create-input'
                    type="text"
                    placeholder="Name"
                    onChange={(e) => setName(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Damage"
                    onChange={(e) => setDamage(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Cooldown"
                    onChange={(e) => setCooldown(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Range"
                    onChange={(e) => setRange(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Special"
                    onChange={(e) => setSpecial(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Heal"
                    onChange={(e) => setHeal(e.target.value)}
                />
                <button className='create-btn' onClick={handleCreateAbility}>Create an ability</button>
            </div>
            <h1 className='i-a'>Update an ability</h1>
            <div className='update-ability'>
                <input 
                    className='create-input' 
                    type="number" 
                    placeholder="Enter ability ID" 
                    onChange={e => setAbilityId(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Name"
                    onChange={(e) => setName(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Damage"
                    onChange={(e) => setDamage(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Cooldown"
                    onChange={(e) => setCooldown(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Range"
                    onChange={(e) => setRange(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Special"
                    onChange={(e) => setSpecial(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Heal"
                    onChange={(e) => setHeal(e.target.value)}
                />
                <button className='create-btn' onClick={handleUpdateAbility}>Edit an ability</button>
            </div>
            <h1 className='i-a'>Admin: Delete an ability</h1>
            <div className='delete-ability'>
                <input type="number" placeholder="Enter ability ID" onChange={e => setAbilityId(e.target.value)} />
                <button onClick={handleDeleteAbility}>Delete a NPC</button>
            </div>
            <h1 className='i-a'>Admin: Assign an ability</h1>
            <div className='assign-ability'>
                <input type="number" placeholder="Id from ability..." onChange={e => setAbilityId(e.target.value)} />
                <input type="number" placeholder="to player with id..." onChange={e => setPlayerId(e.target.value)} />
                <button onClick={handleAssignAbility}>Assign</button>
            </div>
        </div>
    );
};

export default Ability;