import React, { useEffect, useState } from 'react';
import './Player.css';

const Player = () => {
    const [player, setPlayer] = useState([]);
    const [playerId, setPlayerId] = useState('');
    const [itemData, setItemData] = useState([]);
    const [itemName, setItemName] = useState('');

    useEffect(() => {
        fetch('http://localhost:5236/api/Item/GetAllItems')
            .then(response => response.json())
            .then(data => setItemData(data));
    }, []);

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
    
    const handleNPCCall = (npcId) => {
        return fetch(`http://localhost:5236/api/NPC/GetNPC?npcId=${npcId}`)
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                return data;
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    const handleLevelUpPlayer = () => {
        fetch(`http://localhost:5236/api/Player/LevelUp?playerId=${playerId}`, {
            method: 'PUT'
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

    const handleItemPlayer = () => {
        fetch(`http://localhost:5236/api/Player/AddItem?itemName=${itemName}&playerID=${playerId}`, {
            method: 'PUT'
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
        <div className='wrap-p'>
            <div className='all-item-data'> Items around the world: 
                {itemData.map(itemData => (
                        <p>{itemData.item.id} - {itemData.item.properties.name}</p>
                    ))}
            </div>
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
            <h1 className='i-p'>Admin: Delete a player</h1>
            <div className='delete-player'>
                <input type="number" placeholder="Enter player ID" onChange={e => setPlayerId(e.target.value)} />
                <button onClick={handleDeletePlayer}>Delete Player</button>
            </div>
            <h1 className='i-p'>Admin: Level up a player</h1>
            <div className='level-player'>
                <input type="number" placeholder="Enter player ID" onChange={e => setPlayerId(e.target.value)} />
                <button onClick={handleLevelUpPlayer}>Level up!</button>
            </div>
            <h1 className='i-p'>Admin: Give player an item</h1>
            <div className='item-player'>
                <input type="number" placeholder="Enter player ID" onChange={e => setPlayerId(e.target.value)} />
                <input type="text" placeholder="Enter item name" onChange={e => setItemName(e.target.value)} />
                <button onClick={handleItemPlayer}>Give</button>
            </div>
            <h1 className='i-p'>Gamewise: See a possible ally</h1>
            <div className='npc-player'>
                <input type="number" placeholder="Enter npc ID" onChange={e => setPlayerId(e.target.value)} />
                <button onClick={handleNPCCall}>Call</button>
            </div>
        </div>
    );
};

export default Player;