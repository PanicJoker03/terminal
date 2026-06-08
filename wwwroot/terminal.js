const outputEl = document.getElementById('output');
const inputEl = document.getElementById('input');
const sendBtn = document.getElementById('send');

const connection = new signalR.HubConnectionBuilder()
  .withUrl('/terminalHub')
  .withAutomaticReconnect()
  .build();

connection.on('output', (text) => {
  outputEl.innerText += text + '\n';
  outputEl.scrollTop = outputEl.scrollHeight;
});

connection.on('clear', () => {
  outputEl.innerText = '';
});

connection.start().then(() => {
  // start the session on the server
  connection.invoke('StartSession');
});

sendBtn.addEventListener('click', () => {
  const value = inputEl.value || '';
  connection.invoke('SendInput', value);
  inputEl.value = '';
});

inputEl.addEventListener('keydown', (e) => {
  if (e.key === 'Enter') {
    sendBtn.click();
  }
});
