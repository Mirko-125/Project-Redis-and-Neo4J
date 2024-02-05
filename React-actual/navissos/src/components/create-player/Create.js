import { useNavigate } from 'react-router-dom';
import React, { useEffect, useState } from 'react';
import './Create.css';

function Create() {
    const [email, setEmail] = useState('');
    const [name, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [bio, setBio] = useState('');
    const [className, setClassName] = useState('');
    const [classData, setClassData] = useState([]);

    const navigate = useNavigate();
    
    useEffect(() => {
        fetch('http://localhost:5236/api/Class/GetAll')
            .then(response => response.json())
            .then(data => setClassData(data));
    }, []);
    

    const handleCreate = () => {
        const playerData = {
            email: email,
            name: name,
            password: password,
            bio: bio,
            class: className
        };
        const player = JSON.stringify(playerData);
        console.log(player);
        fetch('http://localhost:5236/api/Player', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: player
        })
        .then(response => {
            if (response.ok) {
                // Player created successfully, navigate to title menu
                navigate('/title-menu');
            } else {

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
                {/*<div className='all-class-data'> In-game classes: 
                    {classData.map(classData => (
                            <p>{classData.name}</p>
                        ))}
                    </div>*/}
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
                        placeholder="Bio"
                        value={bio}
                        onChange={(e) => setBio(e.target.value)}
                    />
                    <select
                        className='create-input'
                        value={className}
                        onChange={(e) => setClassName(e.target.value)}
                        style={{
                            width: '320px', 
                            height: '35px', 
                            fontSize: '14px', 
                        }}
                    >
                        {classData.map(classItem => (
                            <option key={classItem.id} value={classItem.name}>{classItem.name}</option>
                        ))}
                    </select>
                    <button className='create-btn' onClick={handleCreate}>Start adventure!</button>
            </div>
        </div>
    );
}

export default Create;