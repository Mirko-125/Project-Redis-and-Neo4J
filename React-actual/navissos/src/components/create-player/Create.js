import { useNavigate } from 'react-router-dom';
import React, { useEffect, useState } from 'react';
import './Create.css';

function Create() {
    const [email, setEmail] = useState('');
    const [name, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [createdAt, setCreatedAt] = useState('');
    const [bio, setBio] = useState('');
    const [classId, setClassId] = useState('');
    const [classData, setClassData] = useState([]);

    const navigate = useNavigate();
    
    useEffect(() => {
        fetch('http://localhost:5236/api/Class/GetAllClasses')
            .then(response => response.json())
            .then(data => setClassData(data));
    }, []);
    

    const handleCreate = () => {
        const playerData = {
            email,
            name,
            password,
            createdAt,
            bio,
            classId
        };

        fetch('http://localhost:5236/api/Player/AddProperPlayer', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(playerData)
        })
        .then(response => {
            if (response.ok) {
                // Player created successfully, navigate to title menu
                navigate('/title-menu');
            } else {
                // Handle error response
                // You can display an error message or perform any other action here
            }
        })
        .catch(error => {
            // Handle network error
            // You can display an error message or perform any other action here
        });
    };

    return (
        <div className="whole-wrap">    
            <div className='create-frame'>
                <div className='all-class-data'> In-game classes: 
                    {classData.map(classData => (
                            <p>{classData.id} - {classData.properties.name}</p>
                        ))}
                </div>
                <h1 className='create-title'>The Elder Scrolls: Navissos</h1>
                    <h2 className='create-subtitle'>Start your journey into a land of nosql databases</h2>
                    <input
                        className='create-input'
                        type="text"
                        placeholder="Email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                    <input
                        className='create-input'
                        type="text"
                        placeholder="Username"
                        value={name}
                        onChange={(e) => setUsername(e.target.value)}
                    />
                    <input
                        className='create-input'
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    <input
                        className='create-input'
                        type="text"
                        placeholder="Created at"
                        value={createdAt}
                        onChange={(e) => setCreatedAt(e.target.value)}
                    />
                    <input
                        className='create-input'
                        type="text"
                        placeholder="Bio"
                        value={bio}
                        onChange={(e) => setBio(e.target.value)}
                    />
                    <input
                        className='create-input'
                        type="text"
                        placeholder="Class ID"
                        value={classId}
                        onChange={(e) => setClassId(e.target.value)}
                    />
                    <button className='create-btn' onClick={handleCreate}>Start adventure!</button>
            </div>
        </div>
    );
}

export default Create;