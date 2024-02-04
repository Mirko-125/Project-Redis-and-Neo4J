import React, { useEffect, useState } from 'react';
import './Ability.css';
import '../../styling/CrudContainer.css'

const Ability = () => {
    const [playerName, setPlayerName] = useState('');
    const [ability, setAbilities] = useState([]);
    const [name, setName] = useState('');
    const [oldName, setOldName] = useState('');
    const [damage, setDamage] = useState(0);
    const [cooldown, setCooldown] = useState(0);
    const [range, setRange] = useState(0);
    const [special, setSpecial] = useState('');
    const [heal, setHeal] = useState(0);

    const createData = () => {
        const abilityData = {
            name: name,
            damage: damage,
            cooldown: cooldown,
            range: range,
            special: special,
            heal: heal
        };
        return abilityData 
    }

    const updateData = () => {
        const abilityData = {...createData(), oldName: oldName};
        return abilityData;
    }

    const handleCreateAbility = () => {
        const abilityData = createData();

        fetch('http://localhost:5236/api/Ability', {
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
        const abilityData = updateData();

        fetch('http://localhost:5236/api/Ability/Update', {
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
        fetch(`http://localhost:5236/api/Ability?abilityName=${name}`, {
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
        fetch(`http://localhost:5236/api/Ability/AssignAbility?abilityName=${name}&playerName=${playerName}`, {
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
        fetch('http://localhost:5236/api/Ability/GetAll')
            .then(response => response.json())
            .then(data => setAbilities(data));
    }, []);
    return (
        <div className='wrap-ap'>
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
                    {ability.name}<br/>
                    Special: {ability.special}<br/>
                    Damage: {ability.damage}<br/>
                    Heal: {ability.heal}<br/>
                    Cooldown: {ability.cooldown}<br/>
                    Range: {ability.range}<br/>
                </button>
            ))}
            </div>
            <div className='crud-container'>
                <div>
                    <h1 className='i-a'>Create an ability</h1>
                    <div className='input-container'>
                        <input
                            type="text"
                            placeholder="Name"
                            onChange={(e) => setName(e.target.value)}
                        />
                        <input
                            type="number"
                            placeholder="Damage"
                            onChange={(e) => setDamage(e.target.value)}
                        />
                        <input
                            type="number"
                            placeholder="Cooldown"
                            onChange={(e) => setCooldown(e.target.value)}
                        />
                        <input
                            type="number"
                            placeholder="Range"
                            onChange={(e) => setRange(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Special"
                            onChange={(e) => setSpecial(e.target.value)}
                        />
                        <input
                            type="number"
                            placeholder="Heal"
                            onChange={(e) => setHeal(e.target.value)}
                        />
                        <button className='green-bg' onClick={handleCreateAbility}>Create an ability</button>
                    </div>
                </div>
                <div>
                    <h1 className='i-a'>Update an ability</h1>
                    <div className='input-container'>
                        <input
                            type="text"
                            placeholder="Name"
                            onChange={(e) => setName(e.target.value)}
                        />
                        <input
                            type="number"
                            placeholder="Damage"
                            onChange={(e) => setDamage(e.target.value)}
                        />
                        <input
                            type="number"
                            placeholder="Cooldown"
                            onChange={(e) => setCooldown(e.target.value)}
                        />
                        <input
                            type="number"
                            placeholder="Range"
                            onChange={(e) => setRange(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Special"
                            onChange={(e) => setSpecial(e.target.value)}
                        />
                        <input
                            type="number"
                            placeholder="Heal"
                            onChange={(e) => setHeal(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Current Name"
                            onChange={(e) => setOldName(e.target.value)}
                        />
                        <button className='blue-bg' onClick={handleUpdateAbility}>Edit an ability</button>
                    </div>
                </div>
                <div>
                    <h1 className='i-a'>Admin: Delete an ability</h1>
                    <div className='input-container'>
                        <input type="text" placeholder="Ability name" onChange={e => setName(e.target.value)} />
                        <button className='red-bg' onClick={handleDeleteAbility}>Delete an Ability</button>
                    </div>
                    <h1 className='i-a'>Admin: Assign an ability</h1>
                    <div className='input-container'>
                        <input type="text" placeholder="Ability name..." onChange={e => setName(e.target.value)} />
                        <input type="text" placeholder="Player name..." onChange={e => setPlayerName(e.target.value)} />
                        <button className='violet-bg' onClick={handleAssignAbility}>Assign</button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Ability;