import React, { useEffect, useState } from 'react';
import './Achievement.css';

const Achievement = () => { 
    const [playerId, setPlayerId] = useState(0);
    const [achievementId, setAchievementId] = useState(0);
    const [achievementName, setAchievementName] = useState('');
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

        fetch('http://localhost:5236/api/Achievement/AddAchievement', {
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
        fetch('http://localhost:5236/api/Achievement/AllAchievements') 
            .then(response => response.json())
            .then(data => setAchievement(data));
    }, []);
    
    const handleUpdateAchievement = () => {
        fetch(`http://localhost:5236/api/Achievement/UpdateAchievement?achievementId=${achievementId}&newName=${achievementName}&newType=${achievementType}&newPoints=${achievementPoints}&newConditions=${achievementConditions}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            }
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
        fetch(`http://localhost:5236/api/Achievement?achievementId=${achievementId}`, {
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
        fetch(`http://localhost:5236/api/Achievement/GiveAchievement?playerId=${playerId}&achievementId=${achievementId}`, {
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
                        Id: [{achievement.id}]<br/>
                        {achievement.properties.name}<br/>
                        Desc: {achievement.properties.conditions}<br/>
                        Reward: {achievement.properties.points}<br/>
                        Type: {achievement.properties.type}<br/>
                    </button>
                ))}
            </div>
            <h1 className='i-a'>Admin: Create an achievement</h1>
            <div className='create-achievement'>
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter achievement name"
                    onChange={(e) => setAchievementName(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter achievement type"
                    onChange={(e) => setAchievementType(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter achievement points"
                    onChange={(e) => setAchievementPoints(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter achievement conditions"
                    onChange={(e) => setAchievementConditions(e.target.value)}
                />
                <button onClick={handleCreateAchievement}>Create Achievement</button>
            </div>
            <h1 className='i-a'>Admin: Edit an achievement</h1>
            <div className='update-achievement'>
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter achievement's id"
                    onChange={(e) => setAchievementId(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter achievement name"
                    onChange={(e) => setAchievementName(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter achievement type"
                    onChange={(e) => setAchievementType(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter achievement points"
                    onChange={(e) => setAchievementPoints(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter achievement conditions"
                    onChange={(e) => setAchievementConditions(e.target.value)}
                />
                <button onClick={handleUpdateAchievement}>Edit Achievement</button>
            </div>
            <h1 className='i-m'>Admin: Remove an achievement</h1>
            <div className='delete-achievement'>
                <input type="number" placeholder="Enter achievement ID" onChange={e => setAchievementId(e.target.value)} />
                <button onClick={handleDeleteAchievement}>Remove achievement</button>
            </div>
            <h1 className='i-m'>Admin: Give an achievement</h1>
            <div className='give-achievement'>
                <input type="number" placeholder="To player" onChange={e => setPlayerId(e.target.value)} />
                <input type="number" placeholder="Give achivement" onChange={e => setAchievementId(e.target.value)} />
                <button onClick={handleGiveAchievment}>Give</button>
            </div>
        </div>
    );
};

export default Achievement;