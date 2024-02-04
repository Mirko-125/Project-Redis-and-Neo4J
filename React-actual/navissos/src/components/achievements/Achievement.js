import React, { useEffect, useState } from 'react';
import './Achievement.css';
import '../../styling/CrudContainer.css';

const Achievement = () => { 
    const [playerName, setPlayerName] = useState(0);
    const [achievementName, setAchievementName] = useState('');
    const [achievementOldName, setAchievementOldName] = useState('');
    const [achievementType, setAchievementType] = useState('');
    const [achievementPoints, setAchievementPoints] = useState(0);
    const [achievementConditions, setAchievementConditions] = useState('');

    const [achievement, setAchievement] = useState([]);

    const handleCreateAchievement = () => {
        const achievementData = {
            name: achievementName,
            type: achievementType,
            points: achievementPoints,
            conditions: achievementConditions
        };

        fetch('http://localhost:5236/api/Achievement', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(achievementData)
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
        fetch('http://localhost:5236/api/Achievement/GetAll') 
            .then(response => response.json())
            .then(data => setAchievement(data));
    }, []);
    
    const handleUpdateAchievement = () => {
        const achievementData = {
            name: achievementName,
            type: achievementType,
            points: achievementPoints,
            conditions: achievementConditions,
            oldName: achievementOldName
        };

        const body = JSON.stringify(achievementData);

        fetch(`http://localhost:5236/api/Achievement/Update`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: body
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

    const handleDeleteAchievement = () => {
        fetch(`http://localhost:5236/api/Achievement?achievementName=${achievementName}`, {
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

    const handleGiveAchievment = () => {
        fetch(`http://localhost:5236/api/Achievement/GiveAchievement?playerName=${playerName}&achievementName=${achievementName}`, {
            method: 'POST'
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
        <div className='wrap-map-a'>
            <h1 className='i-a'>Achievements</h1>
            <div className='quests'>
                {achievement.map(achievement => (
                    <button key={achievement.id} 
                    style=
                    {{
                        backgroundColor: '#550d0e',
                        backgroundSize: 'cover',
                        width: '200px',
                        height: '100px', 
                        border: 'none',
                        cursor: 'pointer',
                        margin: '3rem',
                        color: 'white'
                    }}>
                        {achievement.name}<br/>
                        Desc: {achievement.conditions}<br/>
                        Reward: {achievement.points}<br/>
                        Type: {achievement.type}<br/>
                    </button>
                ))}
            </div>
            <div className='crud-container'>
                <div>
                    <h1 className='i-a'>Admin: Create an achievement</h1>
                    <div className='input-container'>
                        <input
                            type="text"
                            placeholder="Enter achievement name"
                            onChange={(e) => setAchievementName(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Enter achievement type"
                            onChange={(e) => setAchievementType(e.target.value)}
                        />
                        <input
                            type="number"
                            placeholder="Enter achievement points"
                            onChange={(e) => setAchievementPoints(parseInt(e.target.value))}
                        />
                        <input
                            type="text"
                            placeholder="Enter achievement conditions"
                            onChange={(e) => setAchievementConditions(e.target.value)}
                        />
                        <button className='blue-bg' onClick={handleCreateAchievement}>Create Achievement</button>
                    </div>
                </div>
                <div>
                    <h1 className='i-a'>Admin: Edit an achievement</h1>
                    <div className='input-container'>
                        <input
                            type="text"
                            placeholder="Current name"
                            onChange={(e) => setAchievementOldName(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="New name"
                            onChange={(e) => setAchievementName(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Enter achievement type"
                            onChange={(e) => setAchievementType(e.target.value)}
                        />
                        <input
                            type="number"
                            placeholder="Enter achievement points"
                            onChange={(e) => setAchievementPoints(parseInt(e.target.value))}
                        />
                        <input
                            type="text"
                            placeholder="Enter achievement conditions"
                            onChange={(e) => setAchievementConditions(e.target.value)}
                        />
                        <button className='green-bg' onClick={handleUpdateAchievement}>Edit Achievement</button>
                    </div>
                </div>
                <div>
                    <h1 className='i-m'>Admin: Remove an achievement</h1>
                    <div className='input-container'>
                        <input type="text" placeholder="Enter achievement Name" onChange={e => setAchievementName(e.target.value)} />
                        <button className='red-bg' onClick={handleDeleteAchievement}>Remove achievement</button>
                    </div>
                    <h1 className='i-m'>Admin: Give an achievement</h1>
                    <div className='input-container'>
                        <input type="text" placeholder="To player" onChange={e => setPlayerName(e.target.value)} />
                        <input type="text" placeholder="Give achivement" onChange={e => setAchievementName(e.target.value)} />
                        <button className='cream-bg' onClick={handleGiveAchievment}>Give</button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Achievement;