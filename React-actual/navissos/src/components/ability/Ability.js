import React, { useEffect, useState } from 'react';
import './Ability.css';

const Ability = () => {
    const [ability, setAbilities] = useState([]);
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
        </div>
    );
};

export default Ability;