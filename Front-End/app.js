const API_URL = "http://localhost:5241/api";
const money = new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" });
let products = [];
let cart = [];

// Configurações de Paginação
let currentPage = 1;
const ITEMS_PER_PAGE = 8;

const grid = document.querySelector("#product-grid");
const search = document.querySelector("#product-search");
const categoryFilter = document.querySelector("#category-filter");
const feedback = document.querySelector("#feedback");
const cartItems = document.querySelector("#cart-items");
const emptyCart = document.querySelector("#empty-cart");
const finishButton = document.querySelector("#finish-sale");
const clearButton = document.querySelector("#clear-cart");

// Elementos da Paginação
const prevPageBtn = document.querySelector("#prev-page");
const nextPageBtn = document.querySelector("#next-page");
const pageInfo = document.querySelector("#page-info");

function setFeedback(message, isError = false) { feedback.textContent = message; feedback.classList.toggle("error", isError); }
function cartQuantity(id) { return cart.find(item => item.id === id)?.quantity ?? 0; }
function available(product) { return product.quantidadeEstoque - cartQuantity(product.id); }

// Popula o campo select com as categorias únicas disponíveis nos produtos
function populateCategories() {
  const categories = [...new Set(products.map(p => p.nomeCategoria || "Mercearia"))].sort();
  categoryFilter.innerHTML = '<option value="">Todas as categorias</option>' + 
    categories.map(cat => `<option value="${cat}">${cat}</option>`).join("");
}

function renderProducts(list = products) {
  document.querySelector("#product-count").textContent = `${list.length} produto${list.length === 1 ? "" : "s"} encontrado${list.length === 1 ? "" : "s"}`;
  
  // Cálculo de Paginação
  const totalPages = Math.ceil(list.length / ITEMS_PER_PAGE) || 1;
  if (currentPage > totalPages) currentPage = totalPages;
  
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
  const paginatedList = list.slice(startIndex, startIndex + ITEMS_PER_PAGE);

  grid.innerHTML = paginatedList.map(product => {
    const stock = available(product);
    return `<button class="product-card" data-id="${product.id}" ${stock <= 0 ? "disabled" : ""}>
      <span class="category">${product.nomeCategoria || "Mercearia"}</span><h3>${product.nome}</h3>
      <footer><div><div class="price">${money.format(product.preco)}</div><small class="stock">${stock > 0 ? `${stock} em estoque` : "Sem estoque"}</small></div><span class="add">+</span></footer></button>`;
  }).join("") || "<p>Nenhum produto encontrado.</p>";

  // Atualização dos botões de paginação
  pageInfo.textContent = `Página ${currentPage} de ${totalPages}`;
  prevPageBtn.disabled = currentPage === 1;
  nextPageBtn.disabled = currentPage >= totalPages;
}

function renderCart() {
  const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
  const total = cart.reduce((sum, item) => sum + item.preco * item.quantity, 0);
  
  document.querySelector("#item-badge").textContent = `${totalItems} item${totalItems === 1 ? "" : "s"}`;
  document.querySelector("#subtotal").textContent = money.format(total);
  document.querySelector("#total").textContent = money.format(total);
  
  // Ajuste do carrinho vazio
  emptyCart.style.display = cart.length > 0 ? "none" : "grid";
  emptyCart.hidden = cart.length > 0;

  cartItems.innerHTML = cart.map(item => `<li class="cart-item"><div><h3>${item.nome}</h3><p>${money.format(item.preco)} cada</p></div><div><div class="item-total">${money.format(item.preco * item.quantity)}</div><div class="quantity"><button data-action="decrease" data-id="${item.id}">−</button><strong>${item.quantity}</strong><button data-action="increase" data-id="${item.id}">+</button></div></div></li>`).join("");
  
  finishButton.disabled = !cart.length;
  clearButton.disabled = !cart.length;
  renderProducts(products.filter(matchesSearchAndCategory));
}

function addProduct(id) {
  const product = products.find(product => product.id === id);
  if (!product || available(product) <= 0) return;
  const current = cart.find(item => item.id === id);
  current ? current.quantity++ : cart.push({ ...product, quantity: 1 });
  setFeedback(`${product.nome} adicionado ao carrinho.`); renderCart();
}

function changeQuantity(id, delta) {
  const item = cart.find(item => item.id === id); if (!item) return;
  if (delta > 0 && available(item) <= 0) { setFeedback("Quantidade máxima em estoque atingida.", true); return; }
  item.quantity += delta; if (item.quantity === 0) cart = cart.filter(item => item.id !== id); renderCart();
}

// Filtra combinando busca textual e categoria selecionada
function matchesSearchAndCategory(product) {
  const query = search.value.trim().toLocaleLowerCase();
  const selectedCat = categoryFilter.value;
  
  const matchesSearch = product.nome.toLocaleLowerCase().includes(query) || String(product.id) === query;
  const matchesCategory = !selectedCat || (product.nomeCategoria || "Mercearia") === selectedCat;

  return matchesSearch && matchesCategory;
}

async function loadProducts() {
  setFeedback("Carregando produtos..."); grid.innerHTML = "";
  try {
    const response = await fetch(`${API_URL}/Produtos`);
    if (!response.ok) throw new Error();
    products = (await response.json()).filter(product => product.ativo && product.quantidadeEstoque > 0);
    setFeedback("Digite o nome ou o código do produto e pressione Enter para adicionar.");
    currentPage = 1;
    populateCategories();
    renderProducts(products.filter(matchesSearchAndCategory));
  } catch {
    setFeedback("Não foi possível conectar à API. Confirme se ela está rodando em http://localhost:5080.", true);
    document.querySelector("#product-count").textContent = "Indisponível";
  }
}

// Navegação de páginas
prevPageBtn.addEventListener("click", () => {
  if (currentPage > 1) {
    currentPage--;
    renderProducts(products.filter(matchesSearchAndCategory));
  }
});

nextPageBtn.addEventListener("click", () => {
  currentPage++;
  renderProducts(products.filter(matchesSearchAndCategory));
});

grid.addEventListener("click", event => { const card = event.target.closest("[data-id]"); if (card) addProduct(Number(card.dataset.id)); });
cartItems.addEventListener("click", event => { const button = event.target.closest("button"); if (button) changeQuantity(Number(button.dataset.id), button.dataset.action === "increase" ? 1 : -1); });

search.addEventListener("input", () => {
  currentPage = 1;
  renderProducts(products.filter(matchesSearchAndCategory));
});

categoryFilter.addEventListener("change", () => {
  currentPage = 1;
  renderProducts(products.filter(matchesSearchAndCategory));
});

search.addEventListener("keydown", event => { 
  if (event.key === "Enter") { 
    event.preventDefault(); 
    const exact = products.find(product => matchesSearchAndCategory(product)); 
    if (exact) addProduct(exact.id); 
    else setFeedback("Produto não encontrado.", true); 
  } 
});

document.querySelector("#reload-products").addEventListener("click", loadProducts);
clearButton.addEventListener("click", () => { cart = []; setFeedback("Compra cancelada."); renderCart(); });

finishButton.addEventListener("click", async () => {
  finishButton.disabled = true; finishButton.textContent = "Processando...";
  try {
    const response = await fetch(`${API_URL}/Vendas`, { method:"POST", headers:{"Content-Type":"application/json"}, body:JSON.stringify({ itens:cart.map(item => ({ produtoId:item.id, quantidade:item.quantity })) }) });
    if (!response.ok) throw new Error();
    const sale = await response.json(); showReceipt(sale); cart = []; renderCart(); loadProducts();
  } catch { setFeedback("Não foi possível finalizar a venda. Tente novamente.", true); }
  finally { finishButton.innerHTML = "Finalizar venda <span>→</span>"; finishButton.disabled = !cart.length; }
});

function showReceipt(sale) {
  document.querySelector("#receipt-content").innerHTML = `<p class="eyebrow">VENDA CONCLUÍDA</p><h2>Obrigada pela compra!</h2><p>Pedido #${sale.id} • ${new Date(sale.dataVenda).toLocaleString("pt-BR")}</p>${sale.itens.map(item => `<div class="receipt-row"><span>${item.quantidade}x ${item.nomeProduto}</span><strong>${money.format(item.subTotal)}</strong></div>`).join("")}<div class="receipt-total"><span>Total</span><span>${money.format(sale.valorTotal)}</span></div>`;
  document.querySelector("#receipt-dialog").showModal();
}

loadProducts();