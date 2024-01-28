import React, { useEffect, useState } from 'react';
import './Player.css';

const Player = () => {
    const [player, setPlayer] = useState([]);
    const [playerName, setPlayerName] = useState('');
    const [itemData, setItemData] = useState([]);
    const [itemName, setItemName] = useState('');
    const [experience, setExperience] = useState('');
    const [singlePlayer, setSinglePlayer] = useState([]);

    useEffect(() => {
        fetch('http://localhost:5236/api/Item/GetAll')
            .then(response => response.json())
            .then(data => setItemData(data));
    }, []);

    useEffect(() => {
        fetch('http://localhost:5236/api/Player/GetAll') 
            .then(response => response.json())
            .then(data => setPlayer(data));
    }, []);

    const handleDeletePlayer = () => {
        fetch(`http://localhost:5236/api/Player?playerName=${playerName}`, {
            method: 'DELETE'
        })
            .then(response => response.json())
            .then(data => {
                window.location.reload();
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };
    
    const handlePlayerCall = () => {
        return fetch(`http://localhost:5236/api/Player/GetOne?name=${playerName}`)
            .then(response => response.json())
            .then(data => {
                console.log(data);
                setSinglePlayer(data);
                return data;
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    const handleLevelUpPlayer = () => {
        fetch(`http://localhost:5236/api/Player/AddExperience?playerName=${playerName}&experience=${experience}`, 
        {
            method: 'PUT'
        })
            .then(response => response.json())
            .then(data => {
                window.location.reload();
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    const handleItemPlayer = () => {
        fetch(`http://localhost:5236/api/Player/AddItem?itemName=${itemName}&playerName=${playerName}`, 
        {
            method: 'PUT'
        })
            .then(response => response.json())
            .then(data => {
                window.location.reload();
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
                        <p>{itemData.name}</p>
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
                        Player: {player.name}<br/>
                        Bio: {player.bio}<br/>
                        Honor: {player.honor}<br/>
                        Gold: {player.gold}<br/>
                        Email: {player.email}<br/>
                        Created At: {player.createdAt}<br/>
                        Achievement Points: {player.achievementPoints}<br/>
                    </button>
                ))}
            </div>
            <h1 className='i-p'>Admin: Delete a player</h1>
            <div className='delete-player'>
                <input type="text" placeholder="Enter player's name" onChange={e => setPlayerName(e.target.value)}/>
                <button onClick={handleDeletePlayer}>Delete Player</button>
            </div>
            <h1 className='i-p'>Admin: Level up a player</h1>
            <div className='level-player'>
                <input type="text" placeholder="Enter player's name" onChange={e => setPlayerName(e.target.value)} />
                <input type="number" placeholder="Enter deserved experience" onChange={e => setExperience(e.target.value)} />
                <button onClick={handleLevelUpPlayer}>Level up!</button>
            </div>
            <h1 className='i-p'>Admin: Give player an item</h1>
            <div className='item-player'>
                <input type="text" placeholder="Enter player's name" onChange={e => setPlayerName(e.target.value)} />
                <input type="text" placeholder="Enter item name" onChange={e => setItemName(e.target.value)} />
                <button onClick={handleItemPlayer}>Give</button>
            </div>
            <h1 className='i-p'>Gamewise: See a possible ally</h1>
            <div className='npc-player'>
                <input type="text" placeholder="Enter player's name" onChange={e => setPlayerName(e.target.value)} />
                <button onClick={handlePlayerCall}>Call</button>
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
                        Player: {singlePlayer.name}<br/>
                        Bio: {singlePlayer.bio}<br/>
                        Honor: {singlePlayer.honor}<br/>
                        Gold: {singlePlayer.gold}<br/>
                        Email: {singlePlayer.email}<br/>
                        Created At: {singlePlayer.createdAt}<br/>
                        Achievement Points: {singlePlayer.achievementPoints}<br/>
                    </div>
            </div>
            </div>
        </div>
    );
};

export default Player;