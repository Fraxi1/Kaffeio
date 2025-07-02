document.addEventListener('DOMContentLoaded', () => {
  // Initialize GSAP animations
  initAnimations();
  
  // Initialize charts
  const responseTimeCtx = document.getElementById('responseTimeChart')?.getContext('2d');
  const successRateCtx = document.getElementById('successRateChart')?.getContext('2d');
  
  const responseTimeChart = responseTimeCtx ? initResponseTimeChart(responseTimeCtx) : null;
  const successRateChart = successRateCtx ? initSuccessRateChart(successRateCtx) : null;
  
  // Elements
  const startSimulationBtn = document.getElementById('startSimulation');
  const stopSimulationBtn = document.getElementById('stopSimulation');
  const simulationForm = document.getElementById('simulationForm');
  const datasetSelect = document.getElementById('datasetSelect');
  const recordCountInput = document.getElementById('recordCount');
  const targetUrlInput = document.getElementById('targetUrl');
  const backendUrlInput = document.getElementById('backendUrl');
  const jwtTokenInput = document.getElementById('jwtToken');
  const loginBtn = document.querySelector('a.btn.btn-info');
  
  // Setup navbar navigation
  setupNavigation();
  
  // Check for stored authentication data
  const authToken = localStorage.getItem('auth_token');
  const userData = localStorage.getItem('user_data');
  const storedBackendUrl = localStorage.getItem('backend_url');
  
  // Update UI based on authentication status
  if (authToken && userData) {
    try {
      const user = JSON.parse(userData);
      // Update login button to show logged in status
      if (loginBtn) {
        loginBtn.innerHTML = `
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M20 21V19C20 17.9391 19.5786 16.9217 18.8284 16.1716C18.0783 15.4214 17.0609 15 16 15H8C6.93913 15 5.92172 15.4214 5.17157 16.1716C4.42143 16.9217 4 17.9391 4 19V21" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M12 11C14.2091 11 16 9.20914 16 7C16 4.79086 14.2091 3 12 3C9.79086 3 8 4.79086 8 7C8 9.20914 9.79086 11 12 11Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          ${user.firstName || user.email}
        `;
        loginBtn.title = `Logged in as ${user.email}`;
      }
      
      // Auto-populate JWT token field
      if (jwtTokenInput) {
        jwtTokenInput.value = authToken;
      }
      
      // Auto-populate backend URL if available
      if (backendUrlInput && storedBackendUrl) {
        backendUrlInput.value = storedBackendUrl;
      }
      
      showNotification('Authentication', `Logged in as ${user.email}`, 'info');
    } catch (error) {
      console.error('Error parsing user data:', error);
    }
  }
  const batchSizeInput = document.getElementById('batchSize');
  const intervalInput = document.getElementById('interval');
  const transmissionList = document.getElementById('transmissionList');
  const dataPreview = document.getElementById('dataPreview');
  
  // Stats elements
  const totalRequestsEl = document.getElementById('totalRequests');
  const successRateEl = document.getElementById('successRate');
  const avgResponseTimeEl = document.getElementById('avgResponseTime');
  const dataVolumeEl = document.getElementById('dataVolume');
  
  // Simulation state
  const simulationState = {
    isRunning: false,
    totalRequests: 0,
    successfulRequests: 0,
    failedRequests: 0,
    responseTimes: [],
    dataVolume: 0,
    simulationInterval: null
  };
  
  // Load available datasets
  loadDatasets();
  
  // Event listeners
  if (startSimulationBtn) startSimulationBtn.addEventListener('click', startSimulation);
  if (stopSimulationBtn) stopSimulationBtn.addEventListener('click', stopSimulation);
  
  // Functions
  async function loadDatasets() {
    if (!datasetSelect) return;
    
    try {
      // First try to generate some initial data to ensure datasets are available
      await fetch('/simulator/generate?count=10');
      
      // Now fetch the datasets
      const response = await fetch('/simulator/data');
      if (!response.ok) throw new Error('Failed to load datasets');
      
      const data = await response.json();
      const datasets = Object.keys(data);
      
      // Clear existing options
      datasetSelect.innerHTML = '<option value="">Select a dataset</option>';
      
      // Populate dataset select
      datasets.forEach(dataset => {
        const option = document.createElement('option');
        option.value = dataset;
        option.textContent = dataset;
        datasetSelect.appendChild(option);
      });
      
      if (datasets.length > 0) {
        showNotification('Datasets loaded', `${datasets.length} datasets available`, 'success');
      } else {
        showNotification('No datasets', 'No datasets available. Please generate data first.', 'error');
      }
    } catch (error) {
      console.error('Error loading datasets:', error);
      showNotification('Error', 'Failed to load datasets. Please check the console for details.', 'error');
      
      // Retry after a delay
      setTimeout(loadDatasets, 3000);
    }
  }
  
  // Function to load datasets for the datasets section
  async function loadDatasetsList() {
    const datasetsListContainer = document.getElementById('datasetsList');
    const datasetPreview = document.getElementById('datasetPreview');
    
    if (!datasetsListContainer) return;
    
    // Show loading indicator
    datasetsListContainer.innerHTML = '<div class="loading-indicator">Loading datasets...</div>';
    
    try {
      // First try to generate some initial data to ensure datasets are available
      await fetch('/simulator/generate?count=10');
      
      // Now fetch the datasets
      const response = await fetch('/simulator/data');
      if (!response.ok) throw new Error('Failed to load datasets');
      
      const data = await response.json();
      const datasets = Object.keys(data);
      
      if (datasets.length === 0) {
        datasetsListContainer.innerHTML = '<div class="loading-indicator">No datasets available</div>';
        return;
      }
      
      // Clear container
      datasetsListContainer.innerHTML = '';
      
      // Add each dataset as a clickable item
      datasets.forEach(dataset => {
        const datasetItem = document.createElement('div');
        datasetItem.className = 'dataset-item';
        datasetItem.innerHTML = `
          <div class="dataset-name">${dataset}</div>
          <div class="dataset-action">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M9 18L15 12L9 6" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
        `;
        
        // Add click event to preview the dataset
        datasetItem.addEventListener('click', async () => {
          // Remove active class from all items
          document.querySelectorAll('.dataset-item').forEach(item => {
            item.classList.remove('active');
          });
          
          // Add active class to clicked item
          datasetItem.classList.add('active');
          
          // Show loading in preview
          datasetPreview.textContent = 'Loading dataset preview...';
          
          try {
            // Fetch the specific dataset
            const dataResponse = await fetch(`/simulator/data?dataset=${dataset}`);
            if (!dataResponse.ok) throw new Error('Failed to load dataset');
            
            const datasetData = await dataResponse.json();
            
            // Display the first 3 items or all if less than 3
            const previewData = datasetData.slice(0, 3);
            datasetPreview.textContent = JSON.stringify(previewData, null, 2);
          } catch (error) {
            console.error('Error loading dataset preview:', error);
            datasetPreview.textContent = `Error loading dataset: ${error.message}`;
          }
        });
        
        datasetsListContainer.appendChild(datasetItem);
      });
      
      // Automatically select the first dataset
      if (datasets.length > 0) {
        datasetsListContainer.querySelector('.dataset-item').click();
      }
      
    } catch (error) {
      console.error('Error loading datasets list:', error);
      datasetsListContainer.innerHTML = `<div class="loading-indicator">Error: ${error.message}</div>`;
    }
  }
  
  // Function to check if JWT token is valid
  async function checkAuthentication(jwtToken) {
    if (!jwtToken) {
      return false;
    }
    
    try {
      // Simple check for token format
      const parts = jwtToken.split('.');
      if (parts.length !== 3) {
        return false;
      }
      
      return true;
    } catch (error) {
      console.error('Error checking authentication:', error);
      return false;
    }
  }
  
  async function startSimulation() {
    const dataset = datasetSelect.value;
    const recordCount = parseInt(recordCountInput.value);
    const targetUrl = targetUrlInput.value;
    const jwtToken = jwtTokenInput.value;
    const batchSize = parseInt(batchSizeInput.value);
    const interval = parseInt(intervalInput.value);
    
    if (!dataset) {
      showNotification('Error', 'Please select a dataset', 'error');
      return;
    }
    
    if (!targetUrl) {
      showNotification('Error', 'Please enter a target URL', 'error');
      return;
    }
    
    try {
      // First generate the data
      const generateResponse = await fetch(`/simulator/generate?count=${recordCount}`);
      if (!generateResponse.ok) throw new Error('Failed to generate data');
      
      const generateData = await generateResponse.json();
      showNotification('Success', generateData.message, 'success');
      
      // Get a preview of the generated data
      const previewResponse = await fetch(`/simulator/data?dataset=${dataset}`);
      if (!previewResponse.ok) throw new Error('Failed to get data preview');
      
      const previewData = await previewResponse.json();
      if (previewData.length > 0) {
        dataPreview.textContent = JSON.stringify(previewData.slice(0, 3), null, 2);
      }
      
      // Start the simulation
      simulationState.isRunning = true;
      startSimulationBtn.disabled = true;
      stopSimulationBtn.disabled = false;
      
      // Update UI to show simulation is running
      document.querySelector('.status-indicator').classList.add('online');
      document.querySelector('.status-text').textContent = 'Simulation Running';
      
      // Reset stats
      resetSimulationStats();
      
      // Start sending data in batches
      simulationState.simulationInterval = setInterval(async () => {
        if (!simulationState.isRunning) {
          clearInterval(simulationState.simulationInterval);
          return;
        }
        
        await sendDataBatch(dataset, targetUrl, batchSize);
      }, interval);
      
      const destinationUrl = targetUrl;
      showNotification('Simulation Started', `Sending data to ${destinationUrl} in batches of ${batchSize} every ${interval}ms`, 'success');
    } catch (error) {
      console.error('Error starting simulation:', error);
      showNotification('Error', 'Failed to start simulation. Please check the console for details.', 'error');
    }
  }
  
  function stopSimulation() {
    simulationState.isRunning = false;
    clearInterval(simulationState.simulationInterval);
    
    startSimulationBtn.disabled = false;
    stopSimulationBtn.disabled = true;
    
    // Update UI to show simulation is stopped
    document.querySelector('.status-indicator').classList.remove('online');
    document.querySelector('.status-text').textContent = 'Simulation Stopped';
    
    showNotification('Simulation Stopped', 'The simulation has been stopped', 'success');
  }
  
  async function sendDataBatch(dataset, targetUrl, batchSize) {
    try {
      const startTime = performance.now();
      
      const response = await fetch('/simulator/send', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          url: targetUrl,
          batchSize: batchSize,
          intervalMs: 0 // Send immediately since we're handling the interval in the frontend
        })
      });
      
      const endTime = performance.now();
      const responseTime = endTime - startTime;
      
      simulationState.totalRequests++;
      
      if (response.ok) {
        simulationState.successfulRequests++;
        const responseData = await response.json();
        
        // Calculate approximate data volume (rough estimate)
        const dataSize = JSON.stringify(responseData).length;
        simulationState.dataVolume += dataSize;
        
        // Add to response times
        simulationState.responseTimes.push(responseTime);
        
        // Log the transmission
        logTransmission({
          timestamp: new Date(),
          status: 'success',
          responseTime: responseTime.toFixed(0),
          message: responseData.message
        });
        
        // Update charts
        updateCharts(responseTimeChart, successRateChart, responseTime);
      } else {
        simulationState.failedRequests++;
        
        // Log the error
        logTransmission({
          timestamp: new Date(),
          status: 'error',
          responseTime: responseTime.toFixed(0),
          message: 'Request failed'
        });
        
        // Update charts
        updateCharts(responseTimeChart, successRateChart, responseTime, false);
      }
      
      // Update stats
      updateStats();
    } catch (error) {
      console.error('Error sending data batch:', error);
      
      simulationState.totalRequests++;
      simulationState.failedRequests++;
      
      // Log the error
      logTransmission({
        timestamp: new Date(),
        status: 'error',
        responseTime: '0',
        message: error.message
      });
      
      // Update stats
      updateStats();
      
      // Update charts
      updateCharts(responseTimeChart, successRateChart, 0, false);
    }
  }
  
  async function sendDataToBackend(dataset, backendUrl, batchSize, jwtToken) {
    // Check authentication before attempting to send data
    if (!jwtToken || !(await checkAuthentication(jwtToken))) {
      console.error('Authentication required for sending telemetry data');
      simulationState.isRunning = false;
      clearInterval(simulationState.simulationInterval);
      document.querySelector('.status-indicator').classList.remove('online');
      document.querySelector('.status-text').textContent = 'Simulation Stopped (Auth Error)';
      startSimulationBtn.disabled = false;
      stopSimulationBtn.disabled = true;
      showNotification('Authentication Error', 'Valid JWT token is required to send data to backend API', 'error');
      return;
    }
    
    try {
      const startTime = performance.now();
      
      const response = await fetch('/simulator/send-to-backend', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          backendUrl: backendUrl,
          batchSize: batchSize,
          intervalMs: 0, // Send immediately since we're handling the interval in the frontend
          jwtToken: jwtToken
        })
      });
      
      const endTime = performance.now();
      const responseTime = endTime - startTime;
      
      simulationState.totalRequests++;
      
      if (response.ok) {
        simulationState.successfulRequests++;
        const responseData = await response.json();
        
        // Calculate approximate data volume (rough estimate)
        const dataSize = JSON.stringify(responseData).length;
        simulationState.dataVolume += dataSize;
        
        // Add to response times
        simulationState.responseTimes.push(responseTime);
        
        // Log the transmission
        logTransmission({
          timestamp: new Date(),
          status: 'success',
          responseTime: responseTime.toFixed(0),
          message: `Telemetry sent to backend: ${responseData.endpoint || backendUrl}`
        });
        
        // Update charts
        updateCharts(responseTimeChart, successRateChart, responseTime);
      } else {
        simulationState.failedRequests++;
        
        // Log the error
        logTransmission({
          timestamp: new Date(),
          status: 'error',
          responseTime: responseTime.toFixed(0),
          message: `Failed to send telemetry to backend: ${response.statusText}`
        });
        
        // Update charts
        updateCharts(responseTimeChart, successRateChart, responseTime, false);
      }
      
      // Update stats display
      updateStats();
    } catch (error) {
      console.error('Error sending data to backend:', error);
      
      simulationState.totalRequests++;
      simulationState.failedRequests++;
      
      // Log the error
      logTransmission({
        timestamp: new Date(),
        status: 'error',
        responseTime: '0',
        message: `Error: ${error.message}`
      });
      
      // Update charts with a failed request
      updateCharts(responseTimeChart, successRateChart, 0, false);
      
      // Update stats display
      updateStats();
    }
  }
  
  function resetSimulationStats() {
    simulationState.totalRequests = 0;
    simulationState.successfulRequests = 0;
    simulationState.failedRequests = 0;
    simulationState.responseTimes = [];
    simulationState.dataVolume = 0;
    
    // Clear the transmission list
    transmissionList.innerHTML = '';
    
    // Reset stats display
    updateStats();
    
    // Reset charts
    resetCharts(responseTimeChart, successRateChart);
  }
  
  function updateStats() {
    // Update stats display
    totalRequestsEl.textContent = simulationState.totalRequests;
    
    const successRate = simulationState.totalRequests > 0 
      ? Math.round((simulationState.successfulRequests / simulationState.totalRequests) * 100) 
      : 0;
    successRateEl.textContent = `${successRate}%`;
    
    const avgResponseTime = simulationState.responseTimes.length > 0
      ? Math.round(simulationState.responseTimes.reduce((a, b) => a + b, 0) / simulationState.responseTimes.length)
      : 0;
    avgResponseTimeEl.textContent = `${avgResponseTime}ms`;
    
    const dataVolume = formatBytes(simulationState.dataVolume);
    dataVolumeEl.textContent = dataVolume;
  }
  
  function logTransmission(transmission) {
    const item = document.createElement('div');
    item.className = 'transmission-item';
    
    const time = new Date(transmission.timestamp).toLocaleTimeString();
    
    item.innerHTML = `
      <div class="transmission-info">
        <div class="transmission-time">${time}</div>
        <div class="transmission-message">${transmission.message}</div>
      </div>
      <div class="transmission-status">
        <span class="status-badge status-${transmission.status}">${transmission.status}</span>
        <span>${transmission.responseTime}ms</span>
      </div>
    `;
    
    // Add to the top of the list
    transmissionList.insertBefore(item, transmissionList.firstChild);
    
    // Limit the number of items
    if (transmissionList.children.length > 50) {
      transmissionList.removeChild(transmissionList.lastChild);
    }
    
    // Animate the new item
    gsap.from(item, {
      opacity: 0,
      y: -20,
      duration: 0.3,
      ease: 'power2.out'
    });
  }
  
  function showNotification(title, message, type = 'success') {
    const notificationContainer = document.getElementById('notificationContainer');
    
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    
    notification.innerHTML = `
      <div class="notification-icon">
        ${type === 'success' 
          ? '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M20 6L9 17L4 12" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>' 
          : '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M12 8V12M12 16H12.01M21 12C21 16.9706 16.9706 21 12 21C7.02944 21 3 16.9706 3 12C3 7.02944 7.02944 3 12 3C16.9706 3 21 7.02944 21 12Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>'}
      </div>
      <div class="notification-content">
        <div class="notification-title">${title}</div>
        <div class="notification-message">${message}</div>
      </div>
    `;
    
    notificationContainer.appendChild(notification);
    
    // Animate the notification
    gsap.fromTo(notification, 
      { opacity: 0, x: 50 },
      { opacity: 1, x: 0, duration: 0.3, ease: 'power2.out' }
    );
    
    // Remove the notification after 5 seconds
    setTimeout(() => {
      gsap.to(notification, {
        opacity: 0,
        x: 50,
        duration: 0.3,
        ease: 'power2.in',
        onComplete: () => {
          notification.remove();
        }
      });
    }, 5000);
  }
  
  function formatBytes(bytes, decimals = 2) {
    if (bytes === 0) return '0 Bytes';
    
    const k = 1024;
    const dm = decimals < 0 ? 0 : decimals;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
    
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }
  
  // Chart functions
  function initResponseTimeChart(ctx) {
    return new Chart(ctx, {
      type: 'line',
      data: {
        labels: [],
        datasets: [{
          label: 'Response Time (ms)',
          data: [],
          borderColor: '#6366f1',
          backgroundColor: 'rgba(99, 102, 241, 0.1)',
          borderWidth: 2,
          tension: 0.4,
          fill: true
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: false
          },
          tooltip: {
            mode: 'index',
            intersect: false
          }
        },
        scales: {
          x: {
            display: true,
            title: {
              display: false
            },
            grid: {
              display: false
            }
          },
          y: {
            display: true,
            title: {
              display: false
            },
            suggestedMin: 0
          }
        }
      }
    });
  }
  
  function initSuccessRateChart(ctx) {
    return new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: ['Success', 'Failure'],
        datasets: [{
          data: [0, 0],
          backgroundColor: [
            '#22c55e',
            '#ef4444'
          ],
          borderWidth: 0
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '70%',
        plugins: {
          legend: {
            position: 'bottom'
          }
        }
      }
    });
  }
  
  function updateCharts(responseTimeChart, successRateChart, responseTime, isSuccess = true) {
    // Update response time chart
    const now = new Date();
    const timeLabel = now.toLocaleTimeString('en-US', { hour12: false, hour: '2-digit', minute: '2-digit', second: '2-digit' });
    
    responseTimeChart.data.labels.push(timeLabel);
    responseTimeChart.data.datasets[0].data.push(responseTime);
    
    // Limit the number of data points
    if (responseTimeChart.data.labels.length > 20) {
      responseTimeChart.data.labels.shift();
      responseTimeChart.data.datasets[0].data.shift();
    }
    
    responseTimeChart.update();
    
    // Update success rate chart
    successRateChart.data.datasets[0].data = [
      simulationState.successfulRequests,
      simulationState.failedRequests
    ];
    
    successRateChart.update();
  }
  
  function resetCharts(responseTimeChart, successRateChart) {
    // Reset response time chart
    responseTimeChart.data.labels = [];
    responseTimeChart.data.datasets[0].data = [];
    responseTimeChart.update();
    
    // Reset success rate chart
    successRateChart.data.datasets[0].data = [0, 0];
    successRateChart.update();
  }
  
  // Setup navigation between sections
  function setupNavigation() {
    const navLinks = document.querySelectorAll('nav a');
    const sections = document.querySelectorAll('main > div');
    
    // Add click event listeners to all navigation links
    navLinks.forEach(link => {
      link.addEventListener('click', (e) => {
        e.preventDefault();
        
        // Get the target section id from the href attribute
        const targetId = link.getAttribute('href').substring(1);
        
        // Remove active class from all nav items
        navLinks.forEach(navLink => {
          navLink.parentElement.classList.remove('active');
        });
        
        // Add active class to the clicked nav item
        link.parentElement.classList.add('active');
        
        // Show the corresponding section
        showSection(targetId);
      });
    });
    
    // Function to show a specific section
    function showSection(sectionId) {
      // Hide all sections first
      document.querySelectorAll('.section').forEach(section => {
        section.style.display = 'none';
      });
      document.querySelector('.dashboard-grid').style.display = 'none';
      
      // Show the appropriate section based on the ID
      if (sectionId === 'dashboard') {
        document.querySelector('.dashboard-grid').style.display = 'grid';
      } else if (sectionId === 'datasets') {
        document.getElementById('datasets-section').style.display = 'block';
        loadDatasetsList();
      } else if (sectionId === 'simulation') {
        document.getElementById('simulation-section').style.display = 'block';
      } else if (sectionId === 'settings') {
        document.getElementById('settings-section').style.display = 'block';
      }
    }
  }
  
  // Animation functions
  function initAnimations() {
    // Animate header elements
    gsap.from('.header-title', {
      opacity: 0,
      y: -20,
      duration: 0.8,
      ease: 'power3.out'
    });
    
    gsap.from('.header-actions', {
      opacity: 0,
      y: -20,
      duration: 0.8,
      delay: 0.2,
      ease: 'power3.out'
    });
    
    // Animate cards
    gsap.utils.toArray('.card').forEach((card, i) => {
      gsap.from(card, {
        opacity: 0,
        y: 30,
        duration: 0.8,
        delay: 0.2 + (i * 0.1),
        ease: 'power3.out'
      });
    });
    
    // Animate stat items
    gsap.utils.toArray('.stat-item').forEach((item, i) => {
      gsap.from(item, {
        opacity: 0,
        scale: 0.9,
        duration: 0.5,
        delay: 0.6 + (i * 0.1),
        ease: 'back.out(1.7)'
      });
    });
  }
});
