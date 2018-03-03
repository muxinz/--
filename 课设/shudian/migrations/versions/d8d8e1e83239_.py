"""empty message

Revision ID: d8d8e1e83239
Revises: 1c5aa4cce2c0
Create Date: 2018-01-09 23:27:49.007000

"""
from alembic import op
import sqlalchemy as sa


# revision identifiers, used by Alembic.
revision = 'd8d8e1e83239'
down_revision = '1c5aa4cce2c0'
branch_labels = None
depends_on = None


def upgrade():
    # ### commands auto generated by Alembic - please adjust! ###
    op.add_column('managers', sa.Column('name', sa.String(length=50), nullable=False))
    # ### end Alembic commands ###


def downgrade():
    # ### commands auto generated by Alembic - please adjust! ###
    op.drop_column('managers', 'name')
    # ### end Alembic commands ###
