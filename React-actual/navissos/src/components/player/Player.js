import React, { useEffect, useState } from 'react';
import './Player.css';

const Player = () => {
    const [player, setPlayer] = useState([]);
    const [playerId, setPlayerId] = useState('');

    useEffect(() => {
        fetch('http://localhost:5236/api/Player/AllPlayers') 
            .then(response => response.json())
            .then(data => setPlayer(data));
    }, []);

    const handleDeletePlayer = () => {
        fetch(`http://localhost:5236/api/Player?playerId=${playerId}`, {
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
            <h1 className='i-p'>Players</h1>
            <div className='players'>
                {player.map(player => (
                    <button key={player.id} 
                    style={{
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
                        Player: {player.properties.name}<br/>
                        Bio: {player.properties.bio}<br/>
                        Honor: {player.properties.honor}<br/>
                        Gold: {player.properties.gold}<br/>
                        Id: [{player.id}]<br/>
                        Email: {player.properties.email}<br/>
                        Created At: {player.properties.createdAt}<br/>
                        Achievement Points: {player.properties.achievementPoints}<br/>
                    </button>
                ))}
            </div>
            <h1 className='i-p'>Admin-Delete a player</h1>
            <div className='delete-player'>
                <input type="number" placeholder="Enter player ID" value={playerId} onChange={e => setPlayerId(e.target.value)} />
                <button onClick={handleDeletePlayer}>Delete Player</button>
            </div>
        </div>
    );
};

export default Player;