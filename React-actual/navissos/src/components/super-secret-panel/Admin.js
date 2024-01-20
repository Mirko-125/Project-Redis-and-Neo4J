import React, { useState } from 'react';
import './Admin.css';
import { useNavigate } from 'react-router-dom';

const Admin = () => {
    const [formData, setFormData] = useState({
        name: '',
        email: '',
        password: '',
        id: ''
    });

    let navigate = useNavigate();

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        // Replace 'apiEndpoint' with the actual API endpoint
        fetch('apiEndpoint', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        })
        .then(response => response.json())
        .then(data => {
            // Handle the response from the API
            console.log(data);
        })
        .catch(error => {
            // Handle any errors
            console.error(error);
        });
    };

    const handleDelete = () => {
        // Replace 'apiEndpoint' with the actual API endpoint
        fetch(`apiEndpoint/${formData.id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        })
        .then(response => response.json())
        .then(data => {
            // Handle the response from the API
            console.log(data);
        })
        .catch(error => {
            // Handle any errors
            console.error(error);
        });
    };

    return (
        <div>
            <button onClick={() => navigate('/title-menu')}>Back</button>
            <h1 className='page-title'>Admin board</h1>
            <div className='control-section'>
                <div>
                    <h2>Achievements</h2>
                    <form onSubmit={handleSubmit}>
                        <label htmlFor='name'>Name:</label>
                        <input type='text' id='name' name='name' value={formData.name} onChange={handleChange} />

                        <label htmlFor='email'>Email:</label>
                        <input type='email' id='email' name='email' value={formData.email} onChange={handleChange} />

                        <label htmlFor='password'>Password:</label>
                        <input type='password' id='password' name='password' value={formData.password} onChange={handleChange} />

                        <button type='submit'>Create</button>
                    </form>

                    <h2>Delete Achievement</h2>
                    <form>
                        <label htmlFor='id'>ID:</label>
                        <input type='text' id='id' name='id' value={formData.id} onChange={handleChange} />

                        <button type='button' onClick={handleDelete}>Delete</button>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default Admin;
