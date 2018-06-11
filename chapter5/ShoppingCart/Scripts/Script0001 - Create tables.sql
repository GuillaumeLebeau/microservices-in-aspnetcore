CREATE SCHEMA IF NOT EXISTS shopcart;

CREATE TABLE shopcart.shopping_cart (
  PRIMARY KEY (shopping_cart_id),
  shopping_cart_id SERIAL,
  user_id          BIGINT NOT NULL,
  UNIQUE (shopping_cart_id, user_id)
);

CREATE INDEX idx_shopping_cart_user_id ON shopcart.shopping_cart (user_id);

CREATE TABLE shopcart.shopping_cart_items (
  PRIMARY KEY (shopping_cart_items_id),
  shopping_cart_items_id  SERIAL,
  shopping_cart_id        INT          NOT NULL,
  product_catalog_id      BIGINT       NOT NULL,
  product_name            VARCHAR(100) NOT NULL,
  product_description     VARCHAR(500) NULL,
  amount                  DECIMAL      NOT NULL,
  currency                VARCHAR(5)   NOT NULL,
  FOREIGN KEY (shopping_cart_id) REFERENCES shopcart.shopping_cart (shopping_cart_id)
);

CREATE INDEX idx_shopping_cart_items_shopping_cart_id ON shopcart.shopping_cart_items (shopping_cart_id);

CREATE TABLE shopcart.event_store (
  PRIMARY KEY (event_store_id),
  event_store_id SERIAL,
  name            VARCHAR(100)              NOT NULL,
  occurred_at     TIMESTAMP WITH TIME ZONE  NOT NULL,
  content         TEXT                      NOT NULL
);
