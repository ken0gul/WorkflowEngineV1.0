const tasks = document.querySelectorAll('#sidebar .task');
const content = document.getElementById('droppable-area');
const canvas = document.getElementById('canvas');
const ctx = canvas.getContext('2d');
let draggingDot = null;
let startTask = null;
let endTask = null;
let connections = [];
let currentDraggedTask = null;
let workflowNameFromDb = null;
let workflowId = null;


document.addEventListener("DOMContentLoaded", async() => {
    workflowNameFromDb = null;
    var workflows = await getWorkflowStates();
    console.log(workflows)
    updateWorkflowStatesUI(workflows)
})

// Set canvas size
function setCanvasSize() {
    canvas.width = content.clientWidth;
    canvas.width = content.clientWidth;
    canvas.height = content.clientHeight;
}
setCanvasSize();
window.addEventListener('resize', setCanvasSize);

// Drag and drop tasks from the sidebar
tasks.forEach(task => {
    task.addEventListener('dragstart', (e) => {
        e.dataTransfer.setData('text/plain', null); // For Firefox
        currentDraggedTask = task;
        e.target.classList.add('dragging');
    });

    task.addEventListener('dragend', (e) => {
        e.target.classList.remove('dragging');
    });
});

content.addEventListener('dragover', (e) => {
    e.preventDefault(); // Allow drop
});

content.addEventListener('drop', (e) => {
    e.preventDefault();

    const rect = content.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    if (currentDraggedTask && !currentDraggedTask.parentElement.id.includes("droppable-area")) {
        // Clone the task if it's from the sidebar
        const clonedTask = currentDraggedTask.cloneNode(true);
        clonedTask.setAttribute('data-id', `cloned-task-${generateRandomId()}`)
        clonedTask.style.position = 'absolute';
        clonedTask.style.left = `${x - clonedTask.offsetWidth / 2}px`;
        clonedTask.style.top = `${y - clonedTask.offsetHeight / 2}px`;
        clonedTask.classList.remove('dragging');
        clonedTask.setAttribute('draggable', true);

        clonedTask.addEventListener('dragstart', (e) => {
            e.dataTransfer.setData('text/plain', null); // For Firefox
            currentDraggedTask = clonedTask;
            e.target.classList.add('dragging');
        });

        clonedTask.addEventListener('dragend', (e) => {
            e.target.classList.remove('dragging');
            drawConnections(); // Redraw connections after drag ends
        });

        // Add event listener to dots in the cloned task
        clonedTask.querySelector('.dot').addEventListener('mousedown', (e) => {
            if (e.target.classList.contains('dot')) {
                draggingDot = e.target;
                startTask = e.target.closest('.task');
                e.preventDefault();
            }
        });

        content.appendChild(clonedTask);
        drawConnections(); // Redraw connections after drop
    } else if (currentDraggedTask) {
        // Move the existing task if it's already in the droppable area
        currentDraggedTask.style.left = `${x - currentDraggedTask.offsetWidth / 2}px`;
        currentDraggedTask.style.top = `${y - currentDraggedTask.offsetHeight / 2}px`;
        drawConnections(); // Redraw connections after drop
    }

    currentDraggedTask = null;
});

// Handle dot dragging and connecting
document.addEventListener('mousedown', (e) => {
    if (e.target.classList.contains('dot')) {
        draggingDot = e.target;
        startTask = e.target.closest('.task');
        e.preventDefault();
    }
});

document.addEventListener('mousemove', (e) => {
    if (draggingDot) {
        const rect = content.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        drawConnections(x, y);
    }
});

document.addEventListener('mouseup', (e) => {
    if (draggingDot && startTask) {
        const tasksInContent = content.querySelectorAll('.task');
        tasksInContent.forEach(task => {
            const taskRect = task.getBoundingClientRect();
            const contentRect = content.getBoundingClientRect();
            const x = e.clientX - contentRect.left;
            const y = e.clientY - contentRect.top;

            if (x >= taskRect.left - contentRect.left && x <= taskRect.right - contentRect.left &&
                y >= taskRect.top - contentRect.top && y <= taskRect.bottom - contentRect.top) {
                const endTask = task;
                if (startTask !== endTask) {
                    addConnection(startTask, endTask,x ,y);
                }
            }
        });
    }
    draggingDot = null;
    startTask = null;
    e.preventDefault();
});

function drawConnections(mouseX = null, mouseY = null) {
    ctx.clearRect(0, 0, canvas.width, canvas.height); // Clear previous drawings
    console.log(connections)
    connections.forEach(conn => {
        const startDot = conn.start.querySelector('.dot');
        const endDot = conn.end.querySelector('.dot');
        if (startDot && endDot) {
          
            const startRect = startDot.getBoundingClientRect();
            const endRect = endDot.getBoundingClientRect();
            const contentRect = content.getBoundingClientRect();

            // Calculate positions relative to the content
            const startX = startRect.left + startRect.width / 2 - contentRect.left;
            const startY = startRect.top + startRect.height / 2 - contentRect.top;
            const endX = endRect.left + endRect.width / 2 - contentRect.left;
            const endY = endRect.top + endRect.height / 2 - contentRect.top;
         
            ctx.beginPath();
            ctx.moveTo(startX, startY);
            ctx.lineTo(endX, endY);
            ctx.strokeStyle = 'black';
            ctx.lineWidth = 2;
            ctx.stroke();
        }
    });

    if (draggingDot && mouseX !== null && mouseY !== null) {
        const startDotRect = draggingDot.getBoundingClientRect();
        const contentRect = content.getBoundingClientRect();

        // Calculate the start position
        const startX = startDotRect.left + startDotRect.width / 2 - contentRect.left;
        const startY = startDotRect.top + startDotRect.height / 2 - contentRect.top;


        ctx.beginPath();
        ctx.moveTo(startX, startY);
        ctx.lineTo(mouseX, mouseY);
        ctx.strokeStyle = 'red'; // Temporary color for dragging
        ctx.lineWidth = 2;
        ctx.stroke();
    }
}

function addConnection(startTask, endTask, x, y) {
    const startTaskId = startTask.getAttribute('data-id');
    const endTaskId = endTask.getAttribute('data-id');
    connections.push({
        start: startTask,
        end: endTask,
        startTaskId: startTaskId,
        endTaskId: endTaskId,
        xLoc: x,
        yLoc: y
    });
    drawConnections();
}

document.querySelector(".save-btn").addEventListener('click', saveWorkflow)
async function saveWorkflow() {



    // Filter tasks that are in the droppable area
    const droppableTasks = Array.from(content.querySelectorAll('.task')).map(task => {
        return {
            id: task.getAttribute('data-id'),
             name: task.innerText.split('\n')[0],
            iconHtml: task.innerHTML,
            
                    x: parseFloat(task.style.left),
                        y: parseFloat(task.style.top)
        }
    });


    // Collect only connections related to droppable tasks
    const droppableTaskIds = droppableTasks.map(task => task.id);
    const filteredConnections = connections.filter(conn =>
        droppableTaskIds.includes(conn.startTaskId) && droppableTaskIds.includes(conn.endTaskId)
    );

    //if (workflowId == null) {
    //    taskStates = {
    //        id: '1',
    //        state: 'preparing'
    //    }
    //}
    var taskStates = await pollTaskStates(workflowId);
   

    const workflow = {
        workflowName: workflowNameFromDb ?? `Workflow-${generateRandomId()}`,
        tasks: droppableTasks.map((task, i) => ({
            
            name: task.name,
            x: task.x,
            y: task.y,
            iconHtml: task.iconHtml,
            stateDTO: taskStates ? taskStates[i]?.state : 'preparing'
        })),
        connections: filteredConnections.map(fc => ({
            startTaskId: fc.startTaskId,
            endTaskId: fc.endTaskId,
            xLoc: fc.xLoc,
            yLoc: fc.yLoc
        }))
    };
   
    
    
    console.log(workflow); // Log the payload to check the structure
    try {
        const response = await fetch('/api/Workflow/saveWorkflow', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(workflow)
        });

        if (response.ok) {
            console.log('Workflow saved successfully.');
        } else {
            const errorText = await response.text();
            console.error('Error saving workflow:', errorText);
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

document.querySelectorAll("#workflow-btn").forEach(btn => {
    btn.addEventListener("click", (e) => {
        connections = []
        content.querySelectorAll('.task').forEach(tEl => {
            content.removeChild(tEl)
        })
        const dataId = e.target.getAttribute("data-workflow-id");
        loadWorkflow(dataId)
        workflowId = dataId;
        pollTaskStates(workflowId);

    });
})


document.querySelector(".start-btn").addEventListener("click", () => {
    startWorkflow(workflowId)
})

// Simulating API call to start the workflow
async function startWorkflow(workflowId) {

    const convertedWorkflowId = parseInt(workflowId);
    try {
        const response = await fetch(`/api/Workflow/start/${convertedWorkflowId}`, {
            method: 'POST',
        });

        if (response.ok) {
            console.log('Workflow started successfully.');
            await pollTaskStates(workflowId);
        } else {
            const errorText = await response.text();
            console.error('Error starting workflow:', errorText);
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

// Polling task states to update UI
async function pollTaskStates(workflowId) {
    if (workflowId == null) return null;
    try {
        const response = await fetch(`/api/Workflow/getStates/${workflowId}`);
        if (response.ok) {
            const taskStates = await response.json();
            updateTaskStatesUI(taskStates, workflowId);
            return taskStates;
        }
    } catch (error) {
        console.error('Error polling task states:', error);
    }

    // Polling interval
    setTimeout(() => pollTaskStates(workflowId), 2000);
}

async function getWorkflowStates() {
    try {
        const response = await fetch("/api/Workflow/getWorkflowStates");
        if (response.ok) {
            const workflowStates = await response.json();
            return workflowStates;
        }
    } catch (err) {
        console.error("Error polling workflow states:", err);
    }
}

// Update UI based on task states
function updateTaskStatesUI(taskStates, wfId) {
    if (wfId !== workflowId) return;

    console.log(taskStates)
    console.log(wfId)
    console.log(workflowId)

    taskStates.forEach(task => {
        const taskElement = document.querySelector(`.task[data-id="${task.name}"]`);
        console.log(task.name)
        if (taskElement) {
            taskElement.classList.remove('preparing', 'working', 'completed');
            taskElement.classList.add(task.state.toLowerCase());
        }
    });
}

// Update UI based on workflows' states
function updateWorkflowStatesUI(workflows) {
  
    workflows.forEach(w => {
        const workflowEl = document.querySelector(`[data-workflow-name=${w.workflowName}]`)
                 if (w) {
                     switch (w.state) {
                         case 2:
                            
                             workflowEl.classList.remove('preparing', 'working', 'completed');
                             workflowEl.classList.add('completed');
                             break;
                           
                         case 1:
                             workflowEl.classList.remove('preparing', 'working', 'completed');
                             workflowEl.classList.add('working');
                             break;
                         default:
                             workflowEl.classList.remove('preparing', 'working', 'completed');
                             break;


                     }


                 }
             })
           
      
    };
async function loadWorkflow(id) {
    try {
        const response = await fetch(`/api/Workflow/${id}`);
        const workflow = await response.json();
        workflowNameFromDb = workflow.workflowName;
        const taskStates = await pollTaskStates(id);
       
        
        workflow.tasks.forEach(task => {
            createDroppableTaskElement(task);

        });
        updateTaskStatesUI(taskStates, id)
        workflow.connections.forEach(conn => {
            console.log(conn)
            startTask = document.querySelector(`.task[data-id="${conn.startTaskId}"]`);
             endTask = document.querySelector(`.task[data-id="${conn.endTaskId}"]`);
            if (startTask && endTask) {
                addConnection(startTask, endTask, conn.xLoc, conn.yLoc);
                
             
            }
        });
        drawConnections();

    } catch (error) {
        console.error('Error loading workflow:', error);
    }
}
function createDroppableTaskElement(taskItem) {

    const taskElement = document.createElement('div');
    taskElement.classList.add('task');
    taskElement.setAttribute('data-id', taskItem.name);
    taskElement.setAttribute('draggable', true);
    taskElement.style.position = 'absolute';
    taskElement.style.left = `${taskItem.x}px`;
    taskElement.style.top = `${taskItem.y}px`;
    taskElement.innerHTML = `${taskItem.iconHtml}`
    taskElement.addEventListener('dragstart', (e) => {
        e.dataTransfer.setData('text/plain', null); // For Firefox
        currentDraggedTask = taskElement;
        e.target.classList.add('dragging');
    });
    taskElement.addEventListener('dragend', (e) => {
        e.target.classList.remove('dragging');
    });
    content.appendChild(taskElement);

    const dot = taskElement.querySelector('.dot');
    dot.addEventListener('mousedown', (e) => {
        draggingDot = dot;
        startTask = taskElement;
        e.preventDefault();
    });

    document.addEventListener('mousemove', (e) => {
        if (draggingDot) {
            drawConnections(startTask, { x: e.clientX - content.offsetLeft, y: e.clientY - content.offsetTop });
        }
    });

    document.addEventListener('mouseup', async (e) => {
        if (draggingDot) {
            const endTask = document.elementFromPoint(e.clientX, e.clientY).closest('.task');
            if (endTask && startTask !== endTask) {
                await saveConnection(startTask.getAttribute('data-id'), endTask.getAttribute('data-id'));
                addConnection(startTask, endTask, true);
            }
            draggingDot = null;
            drawConnections();
        }
    });
}



function generateRandomId(length = 10) {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * characters.length));
    }
    return result;
}